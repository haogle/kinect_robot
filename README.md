# kinect_robot
![主界面](http://legendary.qiniudn.com/kinect_1.png)
![人脸识别](http://legendary.qiniudn.com/kinect_2.png)
![语音识别](http://legendary.qiniudn.com/kinect_3.png)
![阿凡达](http://legendary.qiniudn.com/kinect_4.png)
![绿屏功能](http://legendary.qiniudn.com/kinect_5.png)

安装要求：
本程序的运行条件

pc 环境 ：win 8.1 x64 支持direct x 11的显卡 use 3.0
硬件条件：
kinect for windows 2.0 
Xbox NUI senser设为默认的录音设备
电脑需要联网，否则语音识别不可用

软件依赖：
vs 2013 ulitimateenglish version
([软件][镜像][Microsoft][Visual Studio 2013 with Update 4][Ultimate][简体中文][商业软件][win][x86_64][MSDN]
http://rs.xidian.edu.cn/forum.php?mod=viewthread&tid=695635)
MSSpeech_SR_en-US_TELE.msi
SpeechPlatformRuntime.msi
MSKinectLangPack_enUS.msi
MSKinectLangPack_enUS.msi
kinect for windows sdk 2.0 
Naudio (使用 nuget安装）
Word2013
([软件][镜像][Microsoft Office 2013 with SP1][专业增强版][简体中文][vol][x64]
http://rs.xidian.edu.cn/forum.php?mod=viewthread&tid=614684)
vs 2013 develop tools for word

以上软件全部默认途径安装。

debug 需要改动的东西
access  百度云的密钥1个月更新一次。
百度云手册里面有方法
对软件预编译
xcopy "$(KINECTSDK20_DIR)Redist\Face\$(Platform)\NuiDatabase" 
允许unsafe的代码存在
最后此软件只可以在64位编译 
对于32位库完全不兼容。不做兼容工作




