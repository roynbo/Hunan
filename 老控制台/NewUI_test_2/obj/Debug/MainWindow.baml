   M S B A M L   `   `   `  ����E  ARB.ToolKit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;,clr-namespace:RB.ToolKit;assembly=RB.ToolKit
RB.ToolKit  5   	   [ WSystem.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089Y@clr-namespace:System.Windows.Forms;assembly=System.Windows.FormsSystem.Windows.Forms 5   	    NewUI_test_2   NewUI_test_2.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @toolkit,clr-namespace:RB.ToolKit;assembly=RB.ToolKit     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6F   Owf@clr-namespace:System.Windows.Forms;assembly=System.Windows.Forms     5   	     � Title$  机器人控制台��5   	   $	��350��6   $	��525��6%    � WindowState$ 	Maximized=�61   .��5      (�  (� Stretch$  fill=�6   .��5	   
   �� $
��Left=�6   $��Stretch=�6-   $
�� 350��6I   $	��525��6W   ��5
      �� 5       j� Stretch$
 Fill=�6    �� ImageSource$&  skin/mainactivity_background.png��6,   .+�5      � $
�� 350��6   $	��525��6!    � RowDefinitions 5      �� 5      $
��0.4* �6$   �� 5      $
��0.6* �6%    � ColumnDefinitions 5      �� 5      $
��0.3* �6'   �� 5      $
��0.7* �6'   .+�5      � $��0��6   $��1��6'    5      �� 5      $
��139* �6+   �� 5      $	��19* �6+   .+�5      7  1System.Windows.Forms.Integration.WindowsFormsHost  $��24,-25,-24.4,25.4q�6'   $��2��6B      Child. 5         System.Windows.Forms.Panel  -   5        
panel1��"  System.Windows.Forms.Control   Margin+  %System.Windows.Forms.PaddingConverter$ 1,1,1,1 5   3   5      5      5      � 5       $��1��6   $��1��6'    5!      �� 5"      $
��0.4* �6+   �� 5#      �� 5$      $
��0.2* �6+   �� 5%      �� 5&      $
��0.2* �6+   �� 5'      �� 5(      $
��0.2* �6+    5*      �� 5+      $
��0.2* �6(   �� 5,      �� 5-      $
��0.2* �6(   �� 5.      $
��0.2* �6(   �� 5/      $
��0.2* �6(   .+�51         RB.ToolKit.RBImageButton  -   5        	  Name$	 	btn_start��51   ,   $��1��6?   $��1��6O   
   	ImagePath$
 /skin/btn_push_up_.png��6]      	ImageSize$ 80Q�6�   $��Center=�6�   $��Center=�6�   6�     52      -   5        $	 btn_halt��52   ,   $��3��6>   $��1��6N   $
 skin/btn_push_up_halt.png��6[   $ 80Q�6�   $��Center=�6�   $��Center=�6�   6�     53      -   5        $	 	btn_reset��53   ,   $��5��6?   $��1��6O   $ 
 skin/btn_push_up_reset.png��6\   $ 80Q�6�   $��Center=�6�   $��Center=�6�   6�   55      � 56      $��1��6   $��0��6'    57      �� 58      $	��61* �6+   �� 59      $
��114* �6+   �� 5:      $	��90* �6+   �� 5;      $
��102* �6+   .+�5>      �� $��状态消息提示��6   
���   �$��3��6C   $��33,50,28.6,15q�6W   .��5?      �� -   5        $��robotStatus��5?   "   
���   �66   5@      5A      � 5B      $��0��6   $��0��6'    5C      �� 5D      $
��0.2* �6+   �� 5E      $
��0.8* �6+    5G      �� 5H      $
��0.3* �6(   �� 5I      $
��0.8* �6(   �� 5J      $
��0.8* �6(   .+�5L      �� $��	指示灯��6   $��1��6,   
���   �$��2��6M   $��31,27.6,-0.4,30q�6a   $��2��6z   .��5M      �  � ShowGridLines .   5N      �� 5O   "   �� 5P   "    5R      �� 5S   "   $
��0.3* �60   �� 5T   "   $
��0.7* �60   �� 5U   "   $
��0.1* �60   .+�5W      �� -   5         ��Name$ led_connect��5W   $   $��skin/red_light.png��69   $��0��6U   $��1��6e   �� 5X      -   5        $ 
led_status��5X   $   $��skin/red_light.png��68   $��1��6T   $��1��6d   (� 5Y      $��0��6&   $��0��66   $��2��6C   $��0,0,56.6,0.4q�6W   .��5Z   "   �� $
��Bold"�6(   $��通信状态��6:   5[      (� 5\      $��1��6&   $��0��66   .��5]   "   �� $
��Bold"�6(   $��机器人状态��6:   5^      5_      5`      5a      5b      5d      5e      5f      