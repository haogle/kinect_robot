   M S B A M L   `   `   `  ����
  Haogle    Haogle.avatar   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  HDFace Basics��5   	   $	��735��5   	   $	��770��6    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value��%�   &	        &	 #     &	 F     '�� i     �� 5   
   $ 	#ff6e6e6e��62   �� 5	   
   $ 	#ff52318f��64   �� 5
   
   $ 	#ff00BCF2��62   �� 5   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      5      .��5      � -   5        $��
layoutGrid��5      $��
10 0 10 10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   ]� $��0��6   $��0 0 0 20q�6!   .+�5      �� $
��Left=�6   $��pics\Logo.png��6*   $
��Fill=�6A   $��32��6P   $��81��6\   $��0 10 0 5q�6g   �� 5      $��Right=�6   $��0 0 -1 0q�6/   $��Bottom=�6B   #��[  6]   $��Segoe UI)�6�   $��18(�6�   avatar6�   �� 5      $��1��6   $��pics\Status.png��6$   $
��None=�6=   $��Center=�6L   $��0 0 0 5q�6i   5      �� 5   
   -   5        $��Center=�5   3   $��2��6P   .��6^   Start Capture6m   �� 5    
   -   5         ��Name$ 
txt_Status��5       $��1��6)   ��67   �� *
StatusText+� 5#   
   $��3��6   ��5%      �� 5&      -   5        MyBrush�� �� ImageSource 5&   .   �� *ImageSource+6U   .+�5)      '� -   5        	 '�Name$	 
viewport3d��5)   p   $��Stretch=�6   $��Stretch=�69   $
��Auto��6U   $
��Auto��6b   
 J� RenderTransformOrigin$
 0.5,0.5&�6�   z�5*      �� 5+       �� ScaleX$	 1.0Q�6%    �� ScaleY$	 1.0Q�62   6A    '� Camera 5/      =� 50       � Position$ 0.0, 0.0, 0.451�6(    � LookDirection$ 0,0, -15�6B    � UpDirection$ 0,1,05�6[    =� FieldOfView$ 70Q�6o   x�54      g� 58       g� Content 59      t� 5:       �� Color$ #FFFFFF��6/    t� 	Direction$ -0.5,-0.5,-0.55�6?   5<      g� 5=       5>      � 5?       � Geometry 5B   "   k� 5C   &   -   5        theGeometry�� k� TriangleIndices �        k� Normals �    k� TextureCoordinates �    k� 	PositionsK �   p/��p/���� p/���� �� �� �� p/���� p/��p/��5G   $    � Material 5K   "   �� 5L   &   .��5M   *   u� -   5        theMaterial�� u� Brush 5N   .   �� 5O   2    �� 
StartPoint$ 0,0.5&�6F    �� EndPoint$ 1,0.5&�6Y   ��5Q   :   �  � Color$ Yellow��6G    � Offset$ 0Q�6V   � 5R   :   $	 Red��6G   $
 0.25Q�6S   � 5S   :   $
 Blue��6G   $
 0.75Q�6T   � 5T   :   $ 	LimeGreen��6G   $ 1Q�6Y   5V   3   5X   +   5Y   '     j� 	Transform  5]   "   �� 5^   &   ! �� Rotation! 5_   *   �� 5`   .   " �� Axis$" 0,3,05�6B   # �� Angle$# 0Q�6O   5b   '   5d      5f      5h      5i      5j      5m      