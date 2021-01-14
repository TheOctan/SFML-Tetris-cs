using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;

namespace Tetris
{
    public class Figure : Drawable
    {
        public Figure(Texture texture)
            : this(texture, new Vector2i(10, 20))
        {

        }
        public Figure(Texture texture, Vector2i size, bool dinamic = true)
        : this(texture, Program.random.Next(0, 7), Program.random.Next(0, 7), new Vector2i(10, 20), dinamic)
        {

        }

        // конструктор, принимает параметром текстуру, номер, цвет фигуры, размер игрового поля
        public Figure(Texture texture, int number, int color, Vector2i size, bool dinamic = true)
        {
            sizeField = size;
            offsetDx = 0;
            dx = 0;
            offset = new Vector2i(0, 0);

            Number = number % 7;
            Color = color;

            timer = 0f;
            delay = 0.3f;

            this.dinamic = dinamic;
            GameOver = false;

            temp = new Vector2i[4];
            TilesPositions = new Vector2i[4];

            sprite = new Sprite(texture);

            if (this.dinamic)
                offsetDx = Program.random.Next(0, sizeField.X - 1);

            for (int i = 0; i < temp.Length; i++)   // расставляем плитки для текущей фигуры
            {
                TilesPositions[i].X = figures[Number, i] % 2 + offsetDx;
                TilesPositions[i].Y = figures[Number, i] / 2;
            }
        }

        public int Color { get => color; set => color = 1 + value % 7; }			// цвет
        public int Number { get; private set; }			// номер
        public bool GameOver { get; private set; }      // конец ли игры
        public Vector2i[] TilesPositions { get; private set; }				// поизции кубиков для текущей фигуры

        private Vector2i[] temp;						// временнае позиций кубиков для текущей фигуры
        private Sprite sprite;							// спрайт
        private Vector2i offset;						// смещение
        private Vector2i sizeField;                     // размер поля

        private int color;								// цвет
        private int dx;									// смещение по горизонтали
        private int offsetDx;							// разброс фигуры по горизонтали
        private int[,] figures = new int[,]				// фигуры
        {
            { 1,5,3,7 },	//I
			{ 2,4,5,7 },	//Z
			{ 3,5,4,6 },	//S
			{ 3,5,4,7 },	//T
			{ 2,5,3,7 },	//L
			{ 3,5,7,6 },	//J
			{ 2,3,4,5 }		//O
		};

        private float timer;                            // таймер
        private float delay;                            // скорость падения

        private bool dinamic;                           // если фигура, динамическая, то она может двигаться, иначе она будет статической

        public void MoveLeft()
        {
	        if (dinamic) dx = -1;
        }				// двигает фигуру влево

        public void MoveRight()
        {
	        if (dinamic) dx = 1;
        }				// двигает фигуру вправо

        public bool Update(float time, int[,] field)
        {
	        if (dinamic) // если фигура динамическая, то обновляем её
	        {
		        bool collision = false;

		        timer += time;  // прибавляем к таймеру прошедшее время

                // <- перемещение по горизонтали ->

		        for (int i = 0; i < TilesPositions.Length; i++)
		        {
			        temp[i] = TilesPositions[i];	// копируем во временные координаты, координаты всех плиток фигуры
			        TilesPositions[i].X += dx;		// двигае фигуру по горизонтали
		        }

		        if (!CheckCollision(field))	// если было столкновение
		        {
			        for (int i = 0; i < 4; i++)
			        {
				        TilesPositions[i] = temp[i];	// то возвращаем в прежнюю позицию
			        }
		        }

		        // падение фигуры

		        if (timer > delay)
		        {
			        for (int i = 0; i < TilesPositions.Length; i++)
			        {
				        temp[i] = TilesPositions[i];    // копируем во временные координаты, координаты всех плиток фигуры
				        TilesPositions[i].Y++;			// смещаем фигуру на единицу вниз
			        }

			        if (!CheckCollision(field))			// если было столкновение
			        {
				        for (int i = 0; i < TilesPositions.Length; i++)
				        {
                            if (temp[i].Y == 1)     // если фигура остановилась под потолком
                                GameOver = true;    // то конец игры
                        }

				        for (int i = 0; i < TilesPositions.Length; i++)
				        {
					        field[temp[i].X, temp[i].Y] = color;	// то закрашиваем поле текущим цветом фигуры
				        }

				        collision = true;
			        }

			        timer = 0;		// сбрасываем таймер
		        }

		        dx = 0;
		        delay = 0.3f;

		        return collision;
	        }

	        return false;
        }				// обновляет фигуру

        public void Draw(RenderTarget target, RenderStates states)
        {
            for (int i = 0; i < TilesPositions.Length; i++)
            {
                sprite.TextureRect = new IntRect(Color * 18, 0, 18, 18);    // устанавливаем нужную плитку соответственно цвету
                sprite.Position = new Vector2f(TilesPositions[i].X * 18, TilesPositions[i].Y * 18) + new Vector2f(28, 31);  // устанавливаем позиции для каждой плитки

                target.Draw(sprite, states);    // отрисовываем плитки в заданную цель
            }
        }               // отрисовка

        public void Reset(int number, int color)
        {
            Number = number;
            this.color = color;

            if (dinamic)
                offsetDx = Program.random.Next(0, sizeField.X - 1);
            
            for (int i = 0; i < temp.Length; i++)   // расставляем плитки для текущей фигуры
            {
                TilesPositions[i].X = figures[Number, i] % 2 + offsetDx;
                TilesPositions[i].Y = figures[Number, i] / 2;
            }

			if(!dinamic) SetPosition(offset);
        }       // сбрасывает фигуру на новую с указанием номера и цвета

        public void Reset()
        {
            Reset(Program.random.Next(0, 7), Program.random.Next(1, 7));
        }       // сбрасывает фигуру на новую с рандомными параметрами номера и цвета

        public void SetPosition(Vector2i position)
        {
            if (!dinamic)
            {
                offset = position;

                for (int i = 0; i < TilesPositions.Length; i++)
                {
                    TilesPositions[i] += offset;
                }
            }
        }       // устанавливает позицию для фигуры

        public void SetPosition(int x, int y)
        {
            SetPosition(new Vector2i(x, y));
        }       // устанавливает позицию для фигуры(перегрузка)

        public void Rotate(int[,] field)
        {
	        if (dinamic)
	        {
		        if (Number != 6)	// если не квадрат(его номер равняется 6), так как квадрат нету смысла поворачивать
		        {
			        Vector2i point = TilesPositions[1];	// точка вращений фигуры
			        for (int i = 0; i < TilesPositions.Length; i++)		// перебираем все плитки фигуры
			        {
						// смена системы координат для фигуры, другими словами поворот фигуры на 90 градусов
				        int x = TilesPositions[i].Y - point.Y;
				        int y = TilesPositions[i].X - point.X;
				        TilesPositions[i].X = point.X - x;
				        TilesPositions[i].Y = point.Y + y;
			        }

			        if (!CheckCollision(field))	// если не было столкновения с краями поля
			        {
				        for (int i = 0; i < TilesPositions.Length; i++)
				        {
					        TilesPositions[i] = temp[i];		// то временные координаты плитки становятся текущими
				        }
			        }
		        }
	        }
        }       // вращает фигуру

        public void Boost()
        {
	        if (dinamic) delay = 0.05f;
        }		// ускоряет падение

        private bool CheckCollision(int[,] field)
        {
	        if (dinamic)
	        {
		        for (int i = 0; i < 4; i++)	// перебираем каждую плитку фигуры
		        {	
			        if (
				        TilesPositions[i].X < 0 ||				// проверка на пересечение с левой границей поля
				        TilesPositions[i].X >= sizeField.X ||	// проверка на пересечение с правой границей поля
				        TilesPositions[i].Y >= sizeField.Y      // проверка на пересечение с нижней границей поля
                    ) return false;

					else if ( 
				        field[
					        TilesPositions[i].X,
					        TilesPositions[i].Y] != 0 
				        )	// или если хотя бы одна из плиток фигуры столкнулась с ненулевой ячейкой игрового поля
				        return false;		// то было солкновение
		        }
	        }

	        return true;
        }				// проверка на столкновение
    }
}
