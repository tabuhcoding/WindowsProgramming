   M S B A M L   `   `   `  ����A  =MyShop, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:MyShopMyShop  5   	        MyShop.pages.InputDatabase   5   	   W SPresentationCore, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=7.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=7.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   &localclr-namespace:MyShop     5   	   -   5          � WindowStartupLocation$  CenterScreen=�5   	    � WindowStyle$
 None=�6.    � AllowsTransparency$
 True��6A   
������  � Title$ InputDatabase��5	   	   $	��500��6   $	��800��6,    � 	Resources 5
      (   "MyShop.RelativeToAbsoluteConverter   absoluteConverter��%   &	          5   
   6,   5      .��5      �  � ColumnDefinitions 5   
   �� 5      $��* �6   �� 5      $
��1.5* �6   .+�5   
   �� -   5        $��closeImageButton��5      $��1��6t    K� ZIndex$ 1��6�     imgClose#��[ 6�   ��6(   �� *
closeImage+ �� 	Converter# [  6�   �� 5   
    �� CornerRadius$ 	10 0 0 10��6   ��5      �� 5      	 �� 
StartPoint$		 0,0&�6&   
 �� EndPoint$	
 1,1&�67   .��5      �  � Color$ Bisque��6#    � Offset$ 0Q�62   � 5      $ #3AADA1��6#   $ 1Q�63   5      ._�5      � .+�5      �� .+�5      �  � Points �    -1��� ;X�����#4EB1B66:   � 5       ' �    �� �� ���� ��
�����#4EB1B66?   A� 5!      $��250 450 0 0q�6   $��40��63   $��40��6>   ���#4EB1B66J   A� 5"      $��
50 400 0 0q�6   $��20��62   $��20��6=   ���#4EB1B66I   5#      �� 5%      $��Center=�6   .+�5'      �� $Q��KEnter your database's name                                to create/connect��6    $
��Wrap=�5(   3   $��Italic$�6G    J� Opacity$	 0.8Q�6Z   $��20 30 20 30q�6h     
NormalText#��[ 6}   �� 5)      $	 100��6   $	��100��60   $	��100��6<   ���#FFFFFF$��15 40 20 20q�6^   ._�5*      A� $��95��6"   $��95��6-   ��5+      �� 5,   "    j� Stretch$ Uniform=�6{    �� ImageSource 6-   �� *	dbImage+# [  6{   5.      5/      50      51      52      �� 53   
   -   5        $��borderDatabase��53      ���#FFFFFF$��1��6<   $ 	0 10 10 0��6L   ._�54      �� $��Center=�6   .+�55      �� $��Enter your database��6   ���#3AB19B  	TitleText#��[ 6L   �� 57      ���#3AB19B$��1q�6/   $
��70 7q�6C   $ 5��6Q   ._�58      � $	��7 9q�6    59      �� 5:      $
��auto �6/   �� 5;      $��* �6/   .+�5>      �� -   5         ��Name$ textDatabase��5>   $   $��Database's Name��6^     textHint#��[ 6u   �� 5?      -   5         ��Name$ txtDatabase��5?   "     textBox#��[ 6^   5@      5A      �� 5B      -   5         ��Name$ connectDatabaseButton��5B      $��Connect��68     
mainButton#��[ 6n   �� 5C      -   5        $ 
backButton��5C      $
��Back��6-   #��[ 6U   �� 5D      $�� ��6   ���#3AB19B#��[ 6:   � 5E      -   5         �Name$ 
loadingBar��5E      $��20��62   $	��150��6>   $y�Hidden=�6J   5F      5G      5H      5I      