// savetest

function TestSaving()
{
	var done = false, saved = false;
	
	var file = OpenFile("test.sav");
	var name = file.read("name", "");
	var age  = file.read("age", 0);
	var height = file.read("height", 0);
	var dead = file.read("dead", true);

    while (!done) {
        sys_font.drawText(0, 0, "Press Spacebar to save to file.");
		
		if (saved) {
			sys_font.drawText(0, 16, "Saved!");
		}
		else {
			sys_font.drawText(0, 16, name);
			sys_font.drawText(0, 32, age);
			sys_font.drawText(0, 48, height);
			sys_font.drawText(0, 64, dead);
		}
		
        FlipScreen();

		while (AreKeysLeft()) {
			var k = GetKey();
			if (k == KEY_ENTER) done = true;
			if (k == KEY_SPACE) {
				file.write("name", "andrew");
				file.write("age", 22);
				file.write("height", 1.778); // meters
				file.write("dead", false);   // you may never know... :P
				file.flush();
				file.close();
				saved = true;
			}
		}
    }
}