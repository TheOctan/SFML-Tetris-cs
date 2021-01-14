using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace Tetris
{
    class Program
    {
	    public static Random random;
	    public static RenderWindow window;
	    public static Game game;

        static void Main(string[] args)
        {
			random = new Random();

			window = new RenderWindow(new VideoMode(320, 480), "Tetris", Styles.Close);     // создаём окно размером 320x480 с надписью Tetris

            window.Closed += Window_Closed;			// подписка на событие закрытия окна

			Image icon = new Image("images/Tetris.png");			// подгружаем картинку иконки
			window.SetIcon(icon.Size.X, icon.Size.Y, icon.Pixels);	// устанавливаем иконку на окно

			// загружаем текстуры
            Texture figureTexture = new Texture("images/tiles.png");
            Texture background = new Texture("images/background.png");
            Texture gameOverTexture = new Texture("images/gameover.png");
            Font progressFont = new Font("fonts/digifaw.TTF");

			game = new Game(
				figureTexture,
				background,
				gameOverTexture,
				progressFont,
				new Vector2i(10, 20)
				);

	        Clock clock = new Clock();	// часы

	        while (window.IsOpen)		// бесконечный цик пока открыто окно
	        {
				window.DispatchEvents();	// обработка событий окна

		        float time = clock.ElapsedTime.AsSeconds();		// получаем текущее время в секундах
		        clock.Restart();								// сбрасываем часы

				game.Control();		// управление игрой
				game.Update(time);	// обновление логики игры

				window.Clear();			// очищаем окно
				game.Render(window);	// рендерим игру
				window.Display();		// отображаем на диплей
	        }
        }

        private static void Window_Closed(object sender, EventArgs e)
        {
            window.Close();
        }	// обработчик события закрытия окна
    }
}
