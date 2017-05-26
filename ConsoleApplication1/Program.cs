using Glc;
using System;
using System.Collections.Generic;

class Program {
    static void Main(string[] args)
	{
		Glance.BuildSetting.outputDir = @"D:\Glc\out\";
		Glance.BuildSetting.sourceDir = @"D:\Glc\src\";
		Glance.BuildSetting.includeDir = @"D:\Glc\include\";
		Glance.BuildSetting.libDir = @"D:\Glc\lib\SFML\";
		Glance.BuildSetting.settingsDir = @"D:\Glc\settings\";
		Glance.BuildSetting.scriptsDir = @"D:\Glc\scripts\";

		Glance.BuildSetting.libs.Add("sfml-graphics.lib");
		Glance.BuildSetting.libs.Add("sfml-window.lib");
		Glance.BuildSetting.libs.Add("sfml-system.lib");
		Glance.BuildSetting.libs.Add("sfml-audio.lib");
		Glance.BuildSetting.libs.Add("sfml-network.lib");

		Glance.BuildSetting.compilerKeys = @"/EHsc " + " /I" + Glance.BuildSetting.sourceDir + " /I" + Glance.BuildSetting.includeDir + @" /Zi";
		Glance.BuildSetting.linkerKeys = @"/LIBPATH:" + Glance.BuildSetting.libDir;

		Glance.BuildSetting.exeName = "main.exe";

		Glance.BuildSetting.isClearSrcDir = true;
		Glance.BuildSetting.isGenerateCode = true;
		Glance.BuildSetting.isRecompile = true;
		Glance.BuildSetting.isRunAppAfterCompiling = true;
		
		Glance.Init();
		//------------------------------------------------

		//script();
		game();
	}
	static void script()
	{
		Glc.Component.Script.CreateFile(@"D:\a.gcs");
	}
	static void game()
	{
		//-------------Client code starts here

		var scene = new Scene();
		var layer1 = new Layer();
		layer1.ClassName = "ObjectsLayer";
		var enemyLayer = new Layer();
		enemyLayer.ClassName = "EnemyLayer";

		var hero = new RenderableObject(new Vec2());
		hero.ClassName = "Hero";
		hero.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\soldier\Soldier1.png");
		hero.AddComponent(new Glc.Component.Script(@"player.gcs"));
		hero.AddComponent(new Glc.Component.Script(@"tag.gcs"));

		var bullet = new RenderableObject(new Vec2());
		bullet.ClassName = "Bullet";
		bullet.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\bullet.jpg");
		bullet.AddComponent(new Glc.Component.Script(@"bullet.gcs"));
		bullet.AddComponent(new Glc.Component.Script(@"tag.gcs"));
		bullet.AddComponent(new Glc.Component.Collider(Glc.Component.Collider.Type.Circle).SetRadius(5));

		Random rand = new Random();

		for (var i = 0; i < 10; ++i)
		{
			var enemy = new Glc.RenderableObject(new Vec2(rand.Next(-400, 400), rand.Next(-300, 300)));
			enemy.GraphComponent = new Glc.Component.GraphicalComponent.Sprite(@"resources\n\1.jpg");
			enemy.AddComponent(new Glc.Component.Collider(Glc.Component.Collider.Type.Rectangle).SetSize(new Vec2(50, 50)));
			enemy.AddComponent(new Glc.Component.Script(@"tag.gcs"));
			enemyLayer.AddObject(enemy);
		}

		layer1.AddObject(hero);
		layer1.AddObject(bullet);
		scene.AddLayer(enemyLayer);
		scene.AddLayer(layer1);
		Glance.AddScene(scene);
		//-------------Client code ends here
		Glance.Build();

		Console.ReadKey();
	}
}