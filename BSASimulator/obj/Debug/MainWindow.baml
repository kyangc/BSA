   M S B A M L   `   `   `  ����  BSASimulator    BSASimulator.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � 
ResizeMode$  NoResize=�5   	    � Title$ 
MainWindow��5   	   $	��231��6   $	��234��6)   .��5      � 
�������$��0,0,4,1q�6   $��Right=�6/   $	��246��6K   .+�5   
   �� $��仿真模式��6   $	��Top=�5      $��91��5	      $��
29,0,116,0q�6   .��5
      �� $��	0,6,-32,7q�6   $
��Left=�5      $��Center=�5      $��93��64   .+�5      	� -   5         	�Name$! SimulationTypeTaRadioButton��5       d� 	IsChecked$
 TrueY�5      {�. $��TA��5      $
��Left=�5      $��Center=�5      	� 5      -   5        $# SimulationTypeTdoaRadioButton��5      {�. $
��TDOA��5      $
��Left=�5      $��Center=�5      	� 5      -   5        $$ SimulationTypeTaaoaRadioButton��5      {�. $��TA+AOA��5      $
��Left=�5      $��Center=�5      	� 5      -   5        $" SimulationTypeRssRadioButton��5      {�. $	��RSS��5      $
��Left=�5       $��Center=�5!      5"      5#      �� 5$   
   $��基站密度��6   $	��Top=�5%      $��91��5&      $��
130,0,10,0q�6   .��5'      �� $��0,0,0,0q�6   $
��Left=�5(      $��Center=�5)      .+�5*      	� -   5        $, &SimulationBsIntensityMostTaRadioButton��5*      {�. $��0.5KM��5,      $
��Left=�5-      $��Center=�5.      	� 5/      -   5        $, &SimulationBsIntensityLessTaRadioButton��5/      {�. $	��1KM��51      $
��Left=�52      $��Center=�53      $
 TrueY�54      	� 55      -   5        $- 'SimulationBsIntensityLeastTaRadioButton��55      {�. $	��3KM��57      $
��Left=�58      $��Center=�59      $
 TrueY�5:      5;      5<      �� 5=   
   $��通信系统��6   $	��Top=�5>      $��55��5?      $��130,96,10,0q�6   .��5@      �� $��0,0,0,0q�6   $
��Left=�5A      $��Center=�5B      .+�5C      	� -   5        $ SystemTypeCdmaRadioButton��5C      {�. $��CDMA2000��5E      $
��Left=�5F      $
 TrueY�5G      $��Center=�5H      	� 5I      -	   5        $  SystemTypeWcdmaRadioButton��5I      {�. $��WCDMA��5K      $
��Left=�5L      $��Center=�5M      5N      5O      �� 5P   
   -
   5        $��StartButton��5Q      $��开始仿真��5R   "   $
��Left=�61   $��	38,99,0,0q�6L   $	��Top=�5S      $��75��6)   $��55��64   � 5T   
   -   5        $��ProgressBar��5U      $
��Left=�5V      $��34��6(   $��
38,160,0,0q�64   $	��Top=�6H   $	��198��6`   5W      5X      