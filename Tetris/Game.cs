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
    public class Game
    {
        private int progress;               // прогресс
        private Text progressText;          // текст прогресса

        private Sprite gameOverSprite;      // спрайт надаиси Game Over
        private Sprite backgroundSprite;    // спрайт фона
        private Sprite figureSprite;        // спрайт фигуры

        private Vector2i sizeField;         // размер игрового поля
        private int[,] field;           // игровое поле
        private Figure active;              // активная фигура
        private Figure generate;            // генерируемая (следующая) фигура

        private bool gameOver;              // конец ли игры

	    private Player player;
	    private Text ScoreText;

        public Game(
            Texture figureTexture,
            Texture background,
            Texture gameOverTexture,
            Font progressFont,
            Vector2i sizeField
        )
        {
			player = new Player()
			{
				Name = "Player1",
				Score = 0
			};
			ScoreText = new Text("", progressFont)
			{
				Position = new Vector2f(5, 430)
			};


            Program.window.KeyPressed += OnWindowKeyPressed;

            progressText = new Text("0", progressFont);
            figureSprite = new Sprite(figureTexture);
            backgroundSprite = new Sprite(background);
            gameOverSprite = new Sprite(gameOverTexture);

            this.sizeField = sizeField;
            field = new int[sizeField.X, sizeField.Y];
            for (int i = 0; i < field.GetLength(0); i++)
            {
                for (int j = 0; j < field.GetLength(1); j++)
                {
                    field[i, j] = 0;    // заполняем матрицу нулями
                }
            }

            active = new Figure(figureTexture, sizeField, true);
            generate = new Figure(figureTexture, sizeField, false);

            gameOver = false;

			progressText.Position = new Vector2f(230, 150);

            gameOverSprite.Scale = new Vector2f(0.5f, 0.5f);
            gameOverSprite.Position += new Vector2f(-10, 50);

            generate.SetPosition(13, 1);
        }

        private void OnWindowKeyPressed(object sender, KeyEventArgs e)
        {
	        switch (e.Code)
	        {
                case Keyboard.Key.Right:	active.MoveRight();		break;
                case Keyboard.Key.Left:		active.MoveLeft();		break;
                case Keyboard.Key.Up:		active.Rotate(field);	break;
	        }
			
        }	// обработчик события нажатия клавиш

        public void Render(RenderTarget target)
        {
            target.Draw(backgroundSprite);	// отрисовывем фон

            for (int i = 0; i < sizeField.X; i++)
            {
                for (int j = 0; j < sizeField.Y; j++)
                {
                    if (field[i, j] == 0) continue;
                    figureSprite.TextureRect = new IntRect(field[i, j] * 18, 0, 18, 18);			// устанваливаем спрайту рамку соответствующего цвету плитки
                    figureSprite.Position = new Vector2f(i * 18, j * 18) + new Vector2f(28, 31);	// утснавливаем позицию плитки на игровом поле

                    target.Draw(figureSprite);		// рисуем плитку в указанную цель(например окно)
                }
            }

            target.Draw(active);				// отрисовываем активную фигуру, которая падает сверху вниз
            target.Draw(generate);				// отрисовываем статическую фигуру, которая показывает следующую генерируемую фигуру
            target.Draw(progressText);			// отриовываем прогресс

	        if (gameOver) // если конец игры
	        {
                target.Draw(gameOverSprite);    // отрисовывае надпись Game Over
				target.Draw(ScoreText);
            }

        }   // рендер игры

        public void Update(float time)
        {
	        gameOver = active.GameOver;

	        if (!active.GameOver && active.Update(time, field))	// если не конец игры и фигура, обновляясь упала уже(метод Update возвращает данное состояние)
	        {
		        if (!active.GameOver) // если при падении фигуры не было конца игры
		        {
			        progress += CheckLines();	// прибавляем к прогрессу количество заполненных линий
			        progressText.DisplayedString = progress.ToString();

					active.Reset(			// активная фигура сбрасывается
						generate.Number,	// её номер
						generate.Color      // и цвет становится генерируемой фигуры
                        );

					generate.Reset();		// а генерируемая фигура сбрасывается рандомно

					ResetLine();			// сбрасываем заполенные линии
		        }
		        else
		        {
                    ScoreText.DisplayedString = player.Name + " Score: " + player.Score.ToString();
		        }
	        }
        }       // обновление логики игры

        public void Control()
        {
			if(Keyboard.IsKeyPressed(Keyboard.Key.Down))	// если нажата клавиша вниз
				active.Boost();								// ускоряем падение фигуры
        }

        public int CheckLines()
        {
	        int lines = 0;			// количество заполненных линий

	        for (int j = 0; j < sizeField.Y; j++)
	        {
		        int horizontal = 0;
		        for (int i = 0; i < sizeField.X; i++)
		        {
			        if (field[i, j] != 0) horizontal++;	// считаем количество кубиков в ряду
		        }

		        if (horizontal == sizeField.X)	// если количество кубиков в ряду равно ширине поля
			        lines++;					// токолиество заполненных линий увеличиваем на 1
	        }

	        return lines;

        }   // проверка заполненных линий

        public void ResetLine()
        {
	        int k = sizeField.Y - 1;

	        for (int i = sizeField.Y - 1; i > 0; i--)		// двигаемся сверху вниз
	        {
		        int count = 0;
		        for (int j = 0; j < sizeField.X; j++)
		        {
			        if (field[j, i] != 0) count++;	// считаем количество кубиков в ряду
			        field[j, k] = field[j, i];		// меняем текущий ряд с указанным k-ым рядом
		        }

		        if (count < sizeField.X) k--;	// если количество кубиков в ряду меньше ширины поля, то ищем следующую позицию по вертикали
	        }

        }	// сброс заполненных линий
    }
}
