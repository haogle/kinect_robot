   M S B A M L   `   `   `  ���� M  ISystem, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089A-clr-namespace:System.IO.Ports;assembly=SystemSystem.IO.Ports  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem  串口助手   串口助手.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     >port-clr-namespace:System.IO.Ports;assembly=System     6sys&clr-namespace:System;assembly=mscorlib       � Title$  机器人调试助手��$	��586��$	��745�� � Icon$3 -/串口助手;component/Images/Clone%20CD.ico�� � WindowStartupLocation$ CenterScreen=� � 	Resources    enParity S� 
MethodName S� 
ObjectType  System.Enum S� MethodParameters M� Type   System.IO.Ports.Parity  	enStopBit   System.IO.Ports.StopBits  DataBits �� Type  BaudRate��%F  &	        &	 (     &	 P     &	 ~     S� $ 	GetValues��"   M� "  S� $ 	GetValues��"   M� "  �� " ��.s���6����7����8���� " ��.s���1200����2400����4800����9600����	14400����	19200����	38400����	56000����	57600����
115200������ 	 �� EndPoint$	 0.5,1&�
 �� 
StartPoint$
 0.5,0&�.���  � Color$ 	#FF1899DC�� � Offset$ 0Q�� $ 	#FF0C9497��$ 1Q�.���  � RowDefinitions �� $��* ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $	��164 ��� $��* �.+��� $��设置��$��3q�.���  �� $��26 ��� $��26 ��� $��26 ��� $��26 ��� $��26 ��� $
��Auto ��� $
��Auto ��� $
��Auto ��� $
��Auto � �� $��* ��� $��69 �.+��� $��0��$��0��.��
端 口：�� -    ��Name$ cmbPortName��$��1��$��0��$��Center=����� *Port.PortNames+�� $��0��$��1��.��波特率：�� -   $ cmbBaudRate��$��1��$��1��$��Center=�����  �� Source# [ �� $��0��$��2��.��数据位：�� -   $ 
cmbDataBit�� �� SelectedIndex$ 2��$��1��$��2��$��Center=� �� SelectedItem �� *Port.DataBits+���� # [ �� $��0��$��3��.��校验位：�� -   $ 	cmbParity��$ 0��$��1��$��3��$��Center=� �� *Port.Parity+���� # [  �� $��0��$��4��.��停止位：�� -   $ 
cmbStopBit��$ 1��$��1��$��4��$��Center=� �� *Port.StopBits+���� # [ d� -    d�Name$ btnOpenPort�� d� 	IsChecked$ FalseY�$��2��$��5��$��36��$��10q�����  �� 
TargetType" d� �� Triggers P�  P� Property �@d�	IsChecked P� Value$
 TrueY�. ���  �� Property � �� �� Value$ 断开机器人��P�  �@d�	IsChecked$ FalseY�. ���  � ��$ 连接机器人���� $��16进制��$��6��$��Center=�$��6q� �� *Port.IsHexData+�� $��显示时间��$��7��$��Center=�$��6q� �� *Port.IsShowDate+�� $��显示发送��$��8��$��Center=�$��6q� �� *Port.IsShowSend+d� $��1��$��6��$��3q�$��Center=� �� *IsStop+���� " d� P�  �@d�	IsChecked$
 TrueY�. ���  � ��$ 继 续��P�  �@d�	IsChecked$ FalseY�. ���  � ��$ 暂 停���� -    ��Name$ btnClearText��$��清 空��$��1��$��7��$��3q�$��Center=��� -   $ btnSave��$��保 存��$��1��$��8��$��3q�$��Center=��� $��接收��$��3��$��2��$	��100��.���� -	    ��Name$ 
txtRecData��" �� VerticalScrollBarVisibility$
 Auto=�$��3q��� $��0��$��2��.+��� -
   $ go��$��go���� -   $
 stop��$
��stop���� -   $
 left��$
��left���� -   $ right��$��right���� -   $ left_u��$��left_u���� -   $ right_u��$��right_u���� $��4��$��3����Z� .@��� $
��10,0q�.���� -   $ btnClearZero��$��	 清 零 ���� $��10,3,0,3q�.���� $��	接收：���� $��0,3,10,3q�.���� ���� *Port.ReceiveDataCounter+�� $��10,3,0,3q�.���� $��接收字节：���� $��0,3,10,3q�.���� ���� *Port.ReceiveBytesCounter+�� $��10,3,0,3q�.���� $��	发送：���� $��0,3,10,3q�.���� ���� *Port.SendDataCounter+�� $��10,3,0,3q�.���� $��发送字节：���� $��0,3,10,3q�.���� ���� *Port.SendBytesCounter+