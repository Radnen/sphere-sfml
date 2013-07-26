// Testing suite:

RequireScript("imagetest.js");
RequireScript("fonttest.js");
RequireScript("windowtest.js");
RequireScript("mousetest.js");
RequireScript("soundtest.js");
RequireScript("surfacetest.js");
RequireScript("spritesettest.js");
RequireScript("savetest.js");

var sys_window = GetSystemWindowStyle();
var sys_arrow = GetSystemArrow();
var sys_font = GetSystemFont();

function game()
{
	if (this.SetScaled) SetScaled(true);

	var done = false;
	var menu = new Menu();
	menu.addOption("Images", TestImages);
	menu.addOption("Fonts", TestFonts);
	menu.addOption("Windows", TestWindows);
	menu.addOption("Spritesets", TestSpritesets);
	menu.addOption("Mouse/Input", TestMouse);
	menu.addOption("Surfaces", TestSurfaces);
	menu.addOption("Music/Sounds", TestMusic);
	menu.addOption("Savefiles", TestSaving);
	menu.addOption("Map Engine", function() {
		CreatePerson("player", "test.rss", false);
		SetUpdateScript("Update();");
		SetRenderScript("Render();");
		MapEngine("village.rmp", 60);
	});
	menu.addOption("Exit", Exit);
	
	while (!done) {
		menu.draw(16, 16, 96, 160);
		
		FlipScreen();
		while (AreKeysLeft()) {
			var key = GetKey();
			menu.update(key);
			if (key == KEY_ESCAPE) done = true;
		}
	}
}

function Render()
{
	sys_font.drawText(0, 0, "Hello world");
}

function Update()
{
	if (IsKeyPressed(KEY_UP)) SetCameraY(-2);
	if (IsKeyPressed(KEY_DOWN)) SetCameraY(2);
	if (IsKeyPressed(KEY_LEFT)) SetCameraX(-2);
	if (IsKeyPressed(KEY_RIGHT)) SetCameraX(2);


	while (AreKeysLeft()) {
		switch (GetKey()) {
			case KEY_ESCAPE: ExitMapEngine(); break;
		}
	}
}

function Menu()
{
	this.items = [];
	this.index = 0;
}

Menu.prototype.addOption = function(name, callback) {
	this.items.push({name: name, callback: callback});
}

Menu.prototype.draw = function(x, y, w, h) {
	sys_window.drawWindow(x, y, w, h);
	
	for (var i = 0; i < this.items.length; ++i) {
		sys_font.drawText(x + 16, y + i*16, this.items[i].name);
	}
	
	sys_arrow.blit(x, y + this.index * 16);
}

Menu.prototype.update = function(key) {
	switch(key) {
		case KEY_UP:
			this.index = Math.max(0, this.index - 1);
		break;
		case KEY_DOWN:
			this.index = Math.min(this.index + 1, this.items.length - 1);
		break;
		case KEY_ENTER:
			this.items[this.index].callback();
		break;
	}
}