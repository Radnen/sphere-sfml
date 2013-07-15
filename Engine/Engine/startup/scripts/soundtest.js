// soundtest

function TestMusic()
{
	var done = false;
	var music = LoadSound("test.ogg");
	var sound = LoadSound("test.wav");
	music.play(true);

    while (!done) {
		sys_font.drawText(0, 0, "test.ogg: " + music.isPlaying());
		
		var pos = Math.floor(music.getPosition());
		var max = Math.floor(music.getLength());
		
		sys_font.drawText(0, 16, pos + "/" + max);
		
		var v = music.getVolume();
		var p = music.getPitch();
		
		sys_font.drawText(0, 32, "Vol: " + v + "/255");
		sys_font.drawText(0, 48, "Pitch: " + p + "/255");
	
		FlipScreen();
	
		while (AreKeysLeft()) {
			switch (GetKey()) {
				case KEY_SPACE:
					if (music.isPlaying())
						music.pause();
					else
						music.play(true);
				break;
				case KEY_ENTER: done = true; break;
				case KEY_CTRL:
					sound.play();
				break;
				case KEY_UP:
					if (v < 300) music.setVolume(v + 1);
				break;
				case KEY_DOWN:
					if (v > 0) music.setVolume(v - 1);
				break;
				case KEY_LEFT:
					if (p > 0.20) music.setPitch(p - 0.20);
				break;
				case KEY_RIGHT:
					if (p < 2) music.setPitch(p + 0.20);
				break;
			}
		}
    }
}