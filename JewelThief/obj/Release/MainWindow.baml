   M S B A M L   `   `   `  ���� E  AJewelThief, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:WizardDungeonWizardDungeon      WizardDungeon.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -localclr-namespace:WizardDungeon     -     � Title$  Haunted Dungeon��$	��600��$��905.5��.��� $��	0,0,22,23q� J� RenderTransformOrigin$ 0.546,0.498&�.+��� -    ��Name$ btnLoad��$��
Load Level��$
��Left=�$��19��$��
416,0,0,14q�$��Bottom=�$��69���� -   $ btnStart��$��Start Level��$
��Left=�$��19��$��
499,0,0,14q�$��Bottom=�$��68��$ 
0.157,0.78&��� -   $ btnSave��$��
Save Level��$
��Left=�$��19��$��
780,0,0,15q�$��Bottom=�$��68��$ 
0.157,0.78&��� -    ��Name$ cvsMainScreen��$	��371��$��44,98,266,0q�$	��Top=��� -    ��Name$ txtLevelDir��$
��Left=�$��19��$��
140,0,0,14q� �� TextWrapping$
 Wrap=�$��..\..\..\LevelNew��$��Bottom=�$	��257���� -   $ levelSave_TxtBox��$
��Left=�$��19��$��656,504,0,14q�$
 Wrap=�$��	level_new��$	��Top=�$	��120���� -    ��Name$ label��$��Level Location:��$
��Left=�$��29��$��27,0,0,7q�$��Bottom=�$��97���� -	   $ lblMsg��$�� ��$
��Left=�$��72��$��	10,10,0,0q�$	��Top=�$	��520��$��36(�$
��Bold"��� -
   $ EnemyLoad_btn��$
��Load��$
��Left=�$��797,331,0,0q�$	��Top=�$��69��$ 
-0.725,0.3&��� -   $ EnemyLoad_textBox��$
 Wrap=�$3��-..\..\..\..\HauntedDungeon\Textures\enemy.png��$��524,331,73,185q� �� 	UndoLimit$ 94��� .+��� -   $ 
FireButton��$
��Left=�$��
37,240,0,0q�$	��Top=�$��40��$��40��
�������
�������
�������.���� -    ��Name$ 	FireImage��$
��Left=�$��40��$	��Top=�$��40���� -   $ GhostButton��$
��Left=�$��
37,170,0,0q�$	��Top=�$��40��$��40��
�������
�������
�������.���� -   $ 
EnemyImage��$
��Left=�$��36��$	��Top=�$��50���� -   $ TWButton��$
��Left=�$��
37,100,0,0q�$	��Top=�$��40��$��40��
�������
�������
�������.���� -   $ 	WallImage��$
��Left=�$��36��$	��Top=�$��50���� -   $ PlayerButton��$
��Left=�$��
37,310,0,0q�$	��Top=�$��40��$��40��
�������
�������
�������.���� -   $ PlayerImage��$
��Left=�$��36��$	��Top=�$��50���� -   $ 
GoalButton��$
��Left=�$��
37,380,0,0q�$	��Top=�$��45��$��40��
�������
�������
�������.���� -   $ 	GoalImage��$
��Left=�$��36��$	��Top=�$��40���� -   $ PowerButton��$
��Left=�$��
37,450,0,0q�$	��Top=�$��45��$��40��
�������
�������
�������.���� -   $ 
PowerImage��$
��Left=�$��36��$	��Top=�$��40���� -   $ 
Resize_btn��$��
Resize Map��$
��Left=�$��637,125,0,0q�$	��Top=�$��75���� -   $ col_txt��$
��Left=�$��23��$
 Wrap=�$	��Top=�$��31��$��601,122,0,0q��� -   $ row_txt��$
��Left=�$��23��$
 Wrap=�$	��Top=�$��31��$��544,122,0,0q��� -   $ R��$��R��$
��Left=�$��527,119,0,0q�$	��Top=��� -   $ C��$��C��$
��Left=�$��584,119,0,0q�$	��Top=��� -   $ Time_TxtBox��$
��Left=�$��22��$
 Wrap=�$	��Top=�$��37��$��788,120,0,0q�$��16(�	 �� TextAlignment$	 Center=��� $
��Time��$
��Left=�$��739,118,0,0q�$	��Top=�$��16(�	� -   
 	�Name$

 Easy��$
��Easy��$
��Left=�$��
703,28,0,0q�$	��Top=� d� 	IsChecked$
 TrueY�	� -   $

 Hard��$
��Hard��$
��Left=�$��
788,28,0,0q�$	��Top=��� -     ��Name$ 	EnemyList��$
��Left=�$��
713,83,0,0q�$	��Top=�$	��120���� -!    ��Name$ 
Invincible��$��
Invincible��$
��Left=�$��
595,83,0,0q�$	��Top=� �� 	ClickMode$ Press=��� -"   $ WallLoad_textBox��$
 Wrap=�$2��,..\..\..\..\HauntedDungeon\Textures\wall.jpg��$��524,279,74,237q�$ 94���� -#   $ WallLoad_btn��$
��Load��$
��Left=�$��797,279,0,0q�$	��Top=�$��69��$ 
-0.725,0.3&��� -$   $ FloorLoad_textBox��$
 Wrap=�$��524,226,73,290q�$ 94��$3��-..\..\..\..\HauntedDungeon\Textures\floor.jpg���� -%   $ PlayerLoad_textBox��$
 Wrap=�$��524,175,74,341q�$ 94��$4��...\..\..\..\HauntedDungeon\Textures\player.bmp���� -&   $ FloorLoad_btn��$
��Load��$
��Left=�$��797,226,0,0q�$	��Top=�$��69��$ 
-0.725,0.3&�$��21���� -'   $ PlayerLoad_btn��$
��Load��$
��Left=�$��797,175,0,0q�$	��Top=�$��69��$ 
-0.725,0.3&�$��21���� -(   $ FireLoad_textBox��$
 Wrap=�$2��,..\..\..\..\HauntedDungeon\Textures\hole.png��$��524,383,74,133q�$ 94���� -)   $ FireLoad_btn��$
��Load��$
��Left=�$��797,383,0,0q�$	��Top=�$��69��$ 
-0.725,0.3&��� -*   $ 
label_Copy��$��Player Icon:��$
��Left=�$��29��$��524,0,0,360q�$��Bottom=�$��97��$ 0.515,-0.724&��� -+   $ label_Copy1��$��Floor Texture:��$
��Left=�$��29��$��524,0,0,312q�$��Bottom=�$��97���� -,   $ label_Copy2��$��Wall Texture:��$
��Left=�$��29��$��524,0,0,261q�$��Bottom=�$��97��$ -0.33,0.379&��� --   $ label_Copy3��$��Enemy Icon:��$
��Left=�$��29��$��524,0,0,208q�$��Bottom=�$��97���� -.   $ label_Copy4��$��
Fire Icon:��$
��Left=�$��29��$��524,0,0,156q�$��Bottom=�$��97���� -/   $ EnemyFreeze��$��Freeze Enemy:��$
��Left=�$��29��$��713,0,0,452q�$��Bottom=�$��97��$
��Bold"��� -0   $ GoalLoad_textBox��$
 Wrap=�$2��,..\..\..\..\HauntedDungeon\Textures\goal.png��$��524,437,74,79q�$ 94���� -1   $ GoalLoad_btn��$
��Load��$
��Left=�$��797,437,0,0q�$	��Top=�$��69��$ 
-0.725,0.3&��� -2   $ GoalIcon_label��$��
Goal Icon:��$
��Left=�$��29��$��524,0,0,104q�$��Bottom=�$��97��