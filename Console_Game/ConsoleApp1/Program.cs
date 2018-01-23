using System;
using System.Linq;
using System.Collections.Generic;

namespace ConsoleApp1 {
    class Exit : GameMapObject {
        public string Mark = "X";
    }

    class Trap : GameMapObject {
        public string Mark = "#";
    }

    class Player : GameMapObject {
        public string Mark = "P";
    }

    class GameMapObject {
        public int xCoordinate;
        public int yCoordinate;

        public string Mark;

        //функция которя генерирует координаты и применяет их к обьекту
        //сгенерировать рандомом в соответствии с максимальными значениями и применить к переменным обьекта
        Tuple<string, int> generateCoordinate(int maxX, int maxY) {
            return Tuple.Create("", 5);
        }
    }

    class Game {
        //константы
        public const int minFieldSideSize = 5;
        public const int maxFieldSideSize = 15;

        //переменные класса
        Player player;
        Exit exit;
        List<Trap> traps;

        int currentFieldWidth;
        int currentFieldHeight;

        string[,] drawing;

        void generateAndStartLevel() {
            Random random = new Random();

            //рассчитать размеры поля
            currentFieldWidth = random.Next(minFieldSideSize, maxFieldSideSize);
            currentFieldHeight = random.Next(minFieldSideSize, maxFieldSideSize);

            //рассчитать количество ловушек
            int trapsCount = Convert.ToInt32(currentFieldWidth * currentFieldHeight * 0.05);
            traps = new List<Trap>();
            //создать ловушки и положить в массив
            for (int i = 0; i < trapsCount; i++) {
                //Создать ловушку
                Trap trap = new Trap();
                //У созданной ловушки вызвать генерацию координат
                //Проверить, нет ли в массиве ловушки с такими же координатами. Если есть, то вызвать генерацию координат ловушки заново. Делать эту процедуру в цикле, так как в теории может несколько раз выдавать повторные координаты.
                bool existing = false;
                int currentXcoord;
                int currentYcoord;
                //Если сгенерированные координаты уникальны - положить ловушку в массив
                do {
                    Random rand = new Random();
                    currentXcoord = rand.Next(currentFieldWidth);
                    currentYcoord = rand.Next(currentFieldHeight);
                    existing = traps.Any(t => t.xCoordinate == currentXcoord && t.yCoordinate == currentYcoord);
                } while (existing);
                trap.xCoordinate = currentXcoord;
                trap.yCoordinate = currentYcoord;
                traps.Add(trap);
            }

            //Создать обьект игрока, сгенерировать для него координаты, проверить, нет ли на этих координатах ловушки(в цикле), и применить этот обьект к переменной player
            player = new Player();
            bool exists = false;
            int currentX;
            int currentY;
            //Если сгенерированные координаты уникальны - положить ловушку в массив
            do {
                Random rand = new Random();
                currentX = rand.Next(currentFieldWidth);
                currentY = rand.Next(currentFieldHeight);
                exists = traps.Any(t => t.xCoordinate == currentX && t.yCoordinate == currentY);
            } while (exists);
            player.xCoordinate = currentX;
            player.yCoordinate = currentY;
            //Создать обьект выхода, проверить нет ли там ловушки или игрока и применить к переменной exit
            exit = new Exit();
            int exitCurrentX;
            int exitCurrentY;
            //Если сгенерированные координаты уникальны - положить ловушку в массив
            do {
                Random rand = new Random();
                exitCurrentX = rand.Next(currentFieldWidth);
                exitCurrentY = rand.Next(currentFieldHeight);
                exists = traps.Any(t => t.xCoordinate == exitCurrentX && t.yCoordinate == exitCurrentY);
                bool playerExists = player.xCoordinate == exitCurrentX && player.yCoordinate == exitCurrentY;
                exists = playerExists || exists;
            } while (exists);
            exit.xCoordinate = exitCurrentX;
            exit.yCoordinate = exitCurrentY;

            //Двумерный символьный массив самого поля
            drawing = new string[currentFieldWidth, currentFieldHeight];
            //отрисовать получившийся массив
            populate();
            Pressing();
        }
        void populate() {
            try {
                drawing[player.xCoordinate, player.yCoordinate] = player.Mark;
                drawing[exit.xCoordinate, exit.yCoordinate] = exit.Mark;
                foreach (Trap trap in traps) drawing[trap.xCoordinate, trap.yCoordinate] = trap.Mark;
                drawField();
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
        //функция отрисовки 2мерного символьного массива
        void drawField() {
            Console.Clear();

            int rowLength = drawing.GetLength(0);
            int colLength = drawing.GetLength(1);

            for (int i = 0; i < rowLength; i++) {
                for (int j = 0; j < colLength; j++) {
                    string value = drawing[i, j];
                    if (value == null) value = "O";
                    Console.Write(string.Format("{0} ", value));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
        }
        public void Pressing() {
        	ConsoleKeyInfo key;
        	do {
        		int xDiff = 0;
        		int yDiff = 0;

				key = Console.ReadKey();
				if (key.Key == ConsoleKey.UpArrow) {
					xDiff = -1;
					yDiff = 0;
				} else if (key.Key == ConsoleKey.DownArrow) {
					xDiff = 1;
					yDiff = 0;
				} else if (key.Key == ConsoleKey.LeftArrow) {
					xDiff = 0;
					yDiff = -1;
				} else if (key.Key == ConsoleKey.RightArrow) {
					xDiff = 0;
					yDiff = 1;
				}  else if (key.Key == ConsoleKey.Escape) {
					Environment.Exit(0);
				}

				int newValueX = player.xCoordinate + xDiff;
				int newValueY = player.yCoordinate + yDiff;

				if (newValueX > currentFieldWidth - 1 || newValueX < 0 || newValueY > currentFieldHeight - 1 || newValueY < 0) {
				//out of range
				} else {
					player.xCoordinate = newValueX;
					player.yCoordinate = newValueY;
				}

				onKeyRedraw();
			} while (key.Key != ConsoleKey.Escape);
		}
        public void existExit(int xPlayer, int yPlayer) {
            if (xPlayer == exit.xCoordinate && yPlayer == exit.yCoordinate) {
                Console.Clear();
                Console.WriteLine("Mission completed!");
                end();
            }
        }
        public void existTrap(int xPlayer, int yPlayer) {
            foreach (Trap trap in traps) {
                if (xPlayer == trap.xCoordinate && yPlayer == trap.yCoordinate) {
                    Console.Clear();
                    Console.WriteLine("Game over");
                    end();
                }
            }
        }
        public void end() {
            Console.Write("Replay? (Y/n)");
            string answer = Console.ReadLine();
            answer.Equals(answer, StringComparison.CurrentCultureIgnoreCase);
            if (answer.Equals("y", StringComparison.CurrentCultureIgnoreCase)) Main();
            if (answer.Equals("n", StringComparison.CurrentCultureIgnoreCase)) Environment.Exit(0);
        }
        public void onKeyRedraw() {
            Console.Clear();
            Array.Clear(drawing, 0, drawing.Length);
            populate();
            drawField();
            existExit(player.xCoordinate, player.yCoordinate);
            existTrap(player.xCoordinate, player.yCoordinate);
        }
        static void Main() {
            Game game = new Game();
            game.generateAndStartLevel();
        }
    }
}