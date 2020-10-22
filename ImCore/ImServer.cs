﻿
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class ImServerExtenssions
{
    static bool isUseWebSockets = false;

    /// <summary>
    /// 启用 ImServer 服务端
    /// </summary>
    /// <param name="app"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseImServer(this IApplicationBuilder app, ImServerOptions options)
    {
        app.Map(options.PathMatch, appcur =>
        {
            var imserv = new ImServer(options);
            if (isUseWebSockets == false)
            {
                isUseWebSockets = true;
                appcur.UseWebSockets(new WebSocketOptions() { KeepAliveInterval = TimeSpan.FromSeconds(10)});
            }
            appcur.Use((ctx, next) =>
                imserv.Acceptor(ctx, next));
        });
        return app;
    }
}

/// <summary>
/// im 核心类实现的配置所需
/// </summary>
public class ImServerOptions : ImClientOptions
{
    /// <summary>
    /// 设置服务名称，它应该是 servers 内的一个
    /// </summary>
    public string Server { get; set; }
}

class ImServer : ImClient
{
    protected string _server { get; set; }

    public ImServer(ImServerOptions options) : base(options)
    {
        _server = options.Server;
        //_redis.Subscribe(($"{_redisPrefix}Server{_server}", RedisSubScribleMessage));
        _redis.Subscribe(($"{_redisPrefix}Server_ImCore", RedisSubScribleMessage));
        //移除在线 
        var c = GetClientListByOnline();
        foreach (var v in c)
        {
            _redis.HDel($"{_redisPrefix}Online", v.ToString());
            //_redis.Eval($"if redis.call('HINCRBY', KEYS[1], '{v}', '-1') <= 0 then redis.call('HDEL', KEYS[1], '{v}') end return 1",
            //$"{_redisPrefix}Online");
        }
        new Thread(_ =>
        {
            while (true)
            {
                try
                {
                    List<WebSocket> list = new List<WebSocket>();
                    foreach (var v in _clients)
                    {
                        foreach (var s in v.Value)
                        {
                            try
                            {
                                var outgoing = new ArraySegment<byte>(Encoding.UTF8.GetBytes(""));
                                CancellationTokenSource source = new CancellationTokenSource(1000);
                                s.Value.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, source.Token).Wait();
                                if(source.IsCancellationRequested)
                                {
                                    Console.WriteLine("超时发送");
                                    list.Add(s.Value.socket);
                                }
                            }
                            catch { 
                                list.Add(s.Value.socket);
                            }
                            if (s.Value.socket.State != WebSocketState.Open)
                            {
                                list.Add(s.Value.socket);
                            }
                        }
                    }
                    foreach (var v in list)
                    {
                        Console.WriteLine("remove websocket");
                        try
                        {
                            v.Abort();
                        }
                        catch { 
                        }
                    }
                }
                catch { }
                Thread.Sleep(100);
            }
        });
    }

    const int BufferSize = 4096;
    ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ImServerClient>> _clients = new ConcurrentDictionary<Guid, ConcurrentDictionary<Guid, ImServerClient>>();

    class ImServerClient
    {
        public WebSocket socket;
        public Guid clientId;

        public ImServerClient(WebSocket socket, Guid clientId)
        {
            this.socket = socket;
            this.clientId = clientId;
        }
    }
    internal async Task Acceptor(HttpContext context, Func<Task> next)
    {
        if (!context.WebSockets.IsWebSocketRequest) return;

        string token = context.Request.Query["token"];
        if (string.IsNullOrEmpty(token)) return;
        var token_value = _redis.Get($"{_redisPrefix}Token{token}");
        if (string.IsNullOrEmpty(token_value))
            throw new Exception("授权错误：用户需通过 ImHelper.PrevConnectServer 获得包含 token 的连接");

        var data = JsonConvert.DeserializeObject<(Guid clientId, string clientMetaData)>(token_value);

        var socket = await context.WebSockets.AcceptWebSocketAsync();

        var cli = new ImServerClient(socket, data.clientId);
        var newid = Guid.NewGuid();

        var wslist = _clients.GetOrAdd(data.clientId, cliid => new ConcurrentDictionary<Guid, ImServerClient>());
        wslist.TryAdd(newid, cli);
        _redis.StartPipe(a => a.HIncrBy($"{_redisPrefix}Online", data.clientId.ToString(), 1).Publish($"evt_{_redisPrefix}Online", token_value));

        Console.WriteLine($"websocket {data.clientId} Online");

        var buffer = new byte[BufferSize];
        var seg = new ArraySegment<byte>(buffer);
        try
        {
            while (socket.State == WebSocketState.Open && _clients.ContainsKey(data.clientId))
            {
                var incoming = await socket.ReceiveAsync(seg, CancellationToken.None);
                var outgoing = new ArraySegment<byte>(buffer, 0, incoming.Count);
            }
            socket.Abort();
        }
        catch
        {
        }
        wslist.TryRemove(newid, out var oldcli);
        if (wslist.Any() == false) _clients.TryRemove(data.clientId, out var oldwslist);
        _redis.Eval($"if redis.call('HINCRBY', KEYS[1], '{data.clientId}', '-1') <= 0 then redis.call('HDEL', KEYS[1], '{data.clientId}') end return 1",
            $"{_redisPrefix}Online");
        Console.WriteLine($"websocket {data.clientId} Offline");
        LeaveChan(data.clientId, GetChanListByClientId(data.clientId));
        _redis.Publish($"evt_{_redisPrefix}Offline", token_value);
    }

    void RedisSubScribleMessage(CSRedis.CSRedisClient.SubscribeMessageEventArgs e)
    {
        try
        {
            var data = JsonConvert.DeserializeObject<(Guid senderClientId, Guid[] receiveClientId, string content, bool receipt)>(e.Body);
            Trace.WriteLine($"收到消息：{data.content}" + (data.receipt ? "【需回执】" : ""));
            Console.WriteLine($"收到消息：{data.content}" + (data.receipt ? "【需回执】" : ""));

            var outgoing = new ArraySegment<byte>(Encoding.UTF8.GetBytes(data.content));
            foreach (var clientId in data.receiveClientId)
            {
                if (_clients.TryGetValue(clientId, out var wslist) == false)
                {
                    //移出 在线列表中的信息

                    Console.WriteLine($"websocket{clientId} 离线了，{data.content}" + (data.receipt ? "【需回执】" : ""));
                    if (data.senderClientId != Guid.Empty && clientId != data.senderClientId && data.receipt)
                        SendMessage(clientId, new[] { data.senderClientId }, new
                        {
                            data.content,
                            receipt = "用户不在线"
                        });
                    continue;
                }

                ImServerClient[] sockarray = wslist.Values.ToArray();

                //如果接收消息人是发送者，并且接收者只有1个以下，则不发送
                //只有接收者为多端时，才转发消息通知其他端
                if (clientId == data.senderClientId && sockarray.Length <= 1) continue;

                foreach (var sh in sockarray)
                    sh.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);

                if (data.senderClientId != Guid.Empty && clientId != data.senderClientId && data.receipt)
                    SendMessage(clientId, new[] { data.senderClientId }, new
                    {
                        data.content,
                        receipt = "发送成功"
                    });
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"订阅方法出错了：{ex.Message}");
        }
    }
}
