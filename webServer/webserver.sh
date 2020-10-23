#!/bin/bash

#拷贝文件
scp -r /C/Users/Administrator/Desktop/im/webServer/bin/Release/netcoreapp3.0/publish/ root@118.31.127.222:/home/docker/aspnetcore/im/webserver

#配置文件
ssh root@118.31.127.222 "cat /home/docker/aspnetcore/im/webserver/publish/appsettings.json"
ssh root@118.31.127.222 "sed -i 's/118.31.127.222:6379/172.16.125.58:6379/g'  /home/docker/aspnetcore/im/webserver/publish/appsettings.json"
ssh root@118.31.127.222 "sed -i 's/118.31.127.222;/172.16.125.58;/g'  /home/docker/aspnetcore/im/webserver/publish/appsettings.json"
ssh root@118.31.127.222 "sed -i 's/127.0.0.1:6001/imserver.kzfuwu.com/g'  /home/docker/aspnetcore/im/webserver/publish/appsettings.json"
ssh root@118.31.127.222 "sed -i 's/ws/wss/g'  /home/docker/aspnetcore/im/webserver/publish/appsettings.json"
ssh root@118.31.127.222 "cat /home/docker/aspnetcore/im/webserver/publish/appsettings.json"
#执行docker更新
ssh root@118.31.127.222 "docker stop kuaizi.im.webserver_0.1"
ssh root@118.31.127.222 "docker rm kuaizi.im.webserver_0.1"
ssh root@118.31.127.222 "docker build /home/docker/aspnetcore/im/webserver -t yl/kuaizi.im.webserver:0.1"
ssh root@118.31.127.222 "docker run --name kuaizi.im.webserver_0.1 -v /etc/localtime:/etc/localtime:ro -p 6002:6002 -d yl/kuaizi.im.webserver:0.1"
#测试
