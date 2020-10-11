#!/bin/bash

#拷贝文件
scp -r /C/Users/Administrator/Desktop/im/imServer/bin/Release/netcoreapp3.0/publish/ root@118.31.127.222:/home/docker/aspnetcore/im/imserver

#配置文件
ssh root@118.31.127.222 "cat /home/docker/aspnetcore/im/imserver/publish/appsettings.json"
ssh root@118.31.127.222 "sed -i 's/118.31.127.222/172.16.125.58/g'  /home/docker/aspnetcore/im/imserver/publish/appsettings.json"
ssh root@118.31.127.222 "cat /home/docker/aspnetcore/im/imserver/publish/appsettings.json"
#执行docker更新
ssh root@118.31.127.222 "docker stop kuaizi.im_0.1"
ssh root@118.31.127.222 "docker rm kuaizi.im_0.1"
ssh root@118.31.127.222 "docker build /home/docker/aspnetcore/im/imserver -t yl/kuaizi.im:0.1"
ssh root@118.31.127.222 "docker run --name kuaizi.im_0.1 -v /etc/localtime:/etc/localtime:ro -p 6001:6001 -d yl/kuaizi.im:0.1"
#测试
