// Testing suite:

RequireScript("imagetest.js");
RequireScript("fonttest.js");
RequireScript("windowtest.js");
RequireScript("mousetest.js");
RequireScript("soundtest.js");
RequireScript("surfacetest.js");
RequireScript("spritesettest.js");

var sys_window = GetSystemWindowStyle();
var sys_arrow = GetSystemArrow();
var sys_font = GetSystemFont();

function game()
{
	var done = false;
	var menu = new Menu();
	menu.addOption("Images", TestImages);
	menu.addOption("Fonts", TestFonts);
	menu.addOption("Windows", TestWindows);
	menu.addOption("Spritesets", TestSpritesets);
	menu.addOption("Mouse/Input", TestMouse);
	menu.addOption("Surfaces", game2);
	menu.addOption("Music/Sounds", TestMusic);
	menu.addOption("Exit", Exit);
	
	while (!done) {
		menu.draw(16, 16, 96, 128);
		FlipScreen();
		while (AreKeysLeft()) {
			var key = GetKey();
			menu.update(key);
			if (key == KEY_ESCAPE) done = true;
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