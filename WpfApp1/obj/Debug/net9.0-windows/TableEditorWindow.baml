   M S B A M L   `   `   `  ����B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.TableEditorWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	   $	��650��5	   	   $	��900��6     � WindowStartupLocation$  CenterScreen=�5
   	   ���	#FF1E1E28$��Segoe UI)�5   	    � Title 5      �� *Title+ � 	Resources 5         BoolToVisibility �� 
TargetType  
SideButton �� Property �� Value �� 
TargetType �� CornerRadius �� Orientation	 �� Triggers
 P� Property P� Value&   System.Windows.Controls.DataGrid=  7System.Windows.Controls.Primitives.DataGridColumnHeader*  $System.Windows.Controls.DataGridCell �� Triggers)  #System.Windows.Controls.DataGridRow��%�	  &	        &	      '  �    '  �    '  �    '  �    �� 5   
   6'   �� 5   
   " ��6   .�5      ��  � �� �	#FF3B3B506+   �� 5       � �� �	#FFE0E0E06+   �� 5       � ��$ 0 0 0 12q�6'   �� 5       � ��$
 12 8q�6(   �� 5       � ��$	 140��6&   �� 5       � ��$
 Left=�6;   �� 5       � ��$ SemiBold"�6+   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      �� #��z��6!   $ 6��5   !   ���	#FF515169$��1q�5    9   ._�5!      �� $ 
Horizontal=�6)   $��Center=�6B   .+�5"   "   �� $	��⮞��6,   $��16(�65   $��0,0,6,0q�6C   $��Center=�6T   �� 5#   "   $��Center=�63   5$      5%      	 5&      P� 5'      
 �@J�IsMouseOver$
 True��6=   . �5(   "   ��  � �� �	#FF56567C6?   5)      P� 5*      
 � ��$
 True��6;   . �5+   "   ��  � �� �	#FF8484A66?   5,      5.      50      51      �� 53   
   "  6   .�54      ��  � �� �	#FF2B2B3B6+   �� 55       � ��
 �����6+   �� 56       �@ RowBackground �	#FF2525356.   �� 57        �@ AlternatingRowBackground �	#FF2E2E4469   �� 58       � �� �	#FF4A4A676,   �� 59       �@ GridLinesVisibility$ 
Horizontal=�64   �� 5:       �@ HeadersVisibility$ Column=�62   �� 5;       �@ CanUserAddRows$
 True��6/   �� 5<       �@ CanUserDeleteRows$
 True��62   �� 5=       �@ 
IsReadOnly$ False��6+   �� 5>       � ��$ 10q�6'   �� 5?       �@ SelectionMode$ Single=�6.   �� 5@       �@ SelectionUnit$ FullRow=�6.   �� 5A       � ��$ 14(�6)   5B      �� 5D   
   "  6   .�5E      ��  � �� �	#FF3D3D5A6+   �� 5F       � ��
 �����6+   �� 5G       � ��$
 10,6q�6(   �� 5H       � �� �	#FF5151696,   �� 5I       � ��$ Center=�6;   �� 5J       � ��$
 Bold"�6+   5K      �� 5M   
   "  6   .�5N      ��  � �� �	#FF5151696,   �� 5O       � ��$ 0,0,1,1q�60    5P      P� 5Q      
 �@ 
IsSelected$
 True��60   . �5R      ��  � �� �	#FF62628E63   �� 5S       � ��
 �����63   5T      5V      �� 5X   
   "  6   .�5Y      ��  � ��$ 30��6'    5Z      P� 5[      
 �@J�IsMouseOver$
 True��61   . �5\      ��  � �� �	#FF41416563   5]      5_      5`      .��5b      � $��12q�6    � RowDefinitions 5c   
   �� 5d      $
��Auto �6   �� 5e      $��* �6    � ColumnDefinitions 5g   
   �� 5h      $	��160 �6   �� 5i      $��* �6   .+�5m   
   �� $��0��6   $��2��6!   ���	#FFE0E0E0$��22(�5n   +   $
��Bold"�69   $��0 0 0 16q�5o      $��Center=�6&   ��5m   5   �� *Title+5o   &   �� 5r   
   $��1��6   $��0��6"   $ Vertical=�62   .+�5s      �� $!��Удалить строку��6   #��[ 6S   ��6.   �� *DeleteRowCommand+6S   �� 5t      $��Импорт из JSON��6   #��[ 6P   ��6.   �� *ImportCommand+6P   �� 5u      $��Экспорт в JSON��6   #��[ 6P   ��6.   �� *ExportCommand+6P   �� 5v      $��Сохранить��6   #��[ 6I   ��6)   �� *SaveCommand+6I   5w        5z   
   $��1��6   $��1��6       AutoGenerateColumns$
 True��6w   ��60   �� *	TableData+ �� SelectedItem 6R   �� *SelectedRow+6w   5{      5|      