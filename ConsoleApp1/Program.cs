using System.Drawing;

namespace Lab1;

class Program
{
    public static int size = 100;
    public static int scale = 3;

    public static Figure[] RandomizeCenters(int classes, Figure[,] figures)
    {
        int x, y;
        Random rand = new Random();
        Figure[] centers = new Figure[classes];
        for (int i = 0; i < classes; i++)
        {
            x = rand.Next(0, (size - 1));
            y = rand.Next(0, (size - 1));
            centers[i] = figures[x, y];
        }

        return centers;
    }

    public static void findCenter(Figure figure, Figure[] centers)
    {
        for (int i = 0; i < centers.Length; i++)
        {
            if (Math.Sqrt(Math.Pow(Math.Abs(figure.x - centers[i].x), 2) +
                          Math.Pow(Math.Abs(figure.y - centers[i].y), 2)) < Math.Sqrt(
                    Math.Pow(Math.Abs(figure.x - centers[figure.classNumber].x), 2) +
                    Math.Pow(Math.Abs(figure.y - centers[figure.classNumber].y), 2)))
            {
                figure.classNumber = i;
            }
        }
    }

    private static bool IsEqual(Figure[] centers, Figure[] temp)
    {
        for (int i = 0; i < centers.Length; i++)
        {
            if (!(centers[i].x == temp[i].x && centers[i].y == temp[i].y &&
                  centers[i].classNumber == temp[i].classNumber)) return true;
        }

        return false;
    }

    private static Figure findNewCenter(Figure[,] figures, Figure oldCenter)
    {
        Figure newCenter = new Figure(0, 0);

        List<Figure> figuresOfClass = new List<Figure>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                if (figures[i, j].classNumber == oldCenter.classNumber)
                {
                    figuresOfClass.Add(figures[i, j]);
                }
            }
        }

        for (int i = 0; i < figuresOfClass.Count; i++)
        {
            newCenter.x += figuresOfClass[i].x;
            newCenter.y += figuresOfClass[i].y;
        }

        newCenter.x /= figuresOfClass.Count;
        newCenter.y /= figuresOfClass.Count;
        
        newCenter.classNumber = oldCenter.classNumber;

        return newCenter;
    }

    public static Bitmap Draw(Figure[,] figures, Figure[] centers)
    {
        Bitmap bmp = new Bitmap(size * scale, size * scale);
        Graphics g = Graphics.FromImage(bmp);

        g.Clear(Color.White);

        Color[] colors = new Color[]
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Yellow,
            Color.Purple,
            Color.Orange,
            Color.Pink,
            Color.Lime,
        };

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int index = figures[i, j].classNumber;
                Color color = colors[index % colors.Length];
                Brush brush = new SolidBrush(color);
                g.FillEllipse(brush, figures[i, j].x * scale, figures[i, j].y * scale, scale, scale);
                brush.Dispose();
            }
        }
        
        Brush blackBrush = new SolidBrush(Color.Black);

        foreach (Figure center in centers)
        {
            g.FillEllipse(blackBrush, center.x * scale, center.y * scale, scale * size / 50, scale * size / 50);
        }
        
        return bmp;
    }

    static void Main()
    {
        int classes;

        Console.Write("Введите количество классов: ");
        while (!int.TryParse(Console.ReadLine(), out classes))
        {
            Console.Write("Вы ввели не число, попробуйте снова: ");
        }

        Figure[,] figures = new Figure[size, size];

        Random rand = new Random();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                int x = rand.Next(0, (size - 1)), y = rand.Next(0, (size - 1)), classNmb = rand.Next(0, (classes - 1));
                figures[i, j] = new Figure(x, y);
                figures[i, j].classNumber = classNmb;
               // Console.WriteLine($"Figure[{i}, {j}]: x={figures[i, j].x}, y={figures[i, j].y}, class= {figures[i, j].classNumber}");
            }
        }

        Figure[] centers = RandomizeCenters(classes, figures), temp = RandomizeCenters(classes, figures);
        
        int iterations = 0;
        
        while (IsEqual(centers, temp))
        {
            if (iterations == 1)
            {
                using (Bitmap b = Draw(figures, centers))
                {
                    b.Save(@"D:\\Учёба\\MaAAS\\ConsoleApp1\\draw1.png");
                }
            }
            
            for (int i = 0; i < classes; i++)
            {
                temp[i] = centers[i];
            }
            
            //1 этап
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    findCenter(figures[i, j], centers);
                    //  Console.WriteLine($"Figure[{i}, {j}]: x={figures[i, j].x}, y={figures[i, j].y}, class= {figures[i, j].classNumber}");

                }
            }

            //2 этап
            for (int i = 0; i < classes; i++)
            {
                centers[i] = findNewCenter(figures, centers[i]);
            }
            
            iterations++;
        }

        Console.WriteLine($"Iterations = {iterations}");
        
        using (Bitmap b = Draw(figures, centers))
        {
            b.Save(@"D:\\Учёба\\MaAAS\\ConsoleApp1\\final.png");
        }
    }
}