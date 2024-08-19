// See https://aka.ms/new-console-template for more information
using System.Collections;

Console.WriteLine("Created by Carlos Munoz");

string[] fileContent;
string[] upperRightCoordinates;
string[,] grid;
Dictionary<int, string> lawnmowers;
void main()
{
    try
    {
        readFile();
        initializeGrid();
        displayMovement();
        displayFinalPositions();
    }
    catch (Exception err)
    {
        Console.WriteLine("unable to complete due to error. Error {0}", err.Message);
    }


}

/*file added to project , change build action to none and output to always
 */
void readFile()
{
    try
    {
        fileContent = File.ReadAllLines("testFile.txt");
        Console.WriteLine("File read succesfully");
    }
    catch (Exception) //err ommited, being thrown
    {
        Console.WriteLine("Error: Unable to read file");
        throw;
    }
}

/*
 * initializes the startup of grid
 */
void initializeGrid()
{
    try
    {
        upperRightCoordinates = fileContent[0].Split(" ");
        //adding + 1 because the first line on the file is not size, is highest coordinate
        upperRightCoordinates[0] = "" + (int.Parse(upperRightCoordinates[0]) + 1);
        upperRightCoordinates[1] = "" + (int.Parse(upperRightCoordinates[1]) + 1);

        grid = new string[int.Parse(upperRightCoordinates[0]), int.Parse(upperRightCoordinates[1])];

        //adding 0 as is easier to read when looking at the grid
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                grid[i, j] = "0 ";

            }
        }
        lawnmowers = new Dictionary<int, string>();
        Console.WriteLine("Grid created");
    }
    catch (Exception)
    {
        Console.WriteLine("Error: Unable to initialize grid");
        throw;
    }
}

/*
 * prints grid on console
 */
void printGrid()
{
    //print it in reverse for row index (0,0) is bottom not top
    for (int row = grid.GetLength(0) - 1; row > -1; row--)
    {
        string[] line = new string[grid.GetLength(0)];

        for (int col = 0; col < grid.GetLength(0); col++)
        {
            line[col] = grid[col, row];

        }
        Console.WriteLine("[" + String.Join(",", line) + "]");
    }
    Console.WriteLine();
}

/*
 * reads next line of file and sets location of lawnmowers
 * rotates lawnmower and moves lawnmower if possible
 * on every move/rotation it shows the grid
 */
void displayMovement()
{
    try
    {
        Console.WriteLine("X = lawnMower, next letter is orientation");
        printGrid();
        //starting at 1 since 0 is the size of the grid
        //jumping by two because 1 line is for position and the next line is for movement

        /*Asumptions:
         * position 0,0 is NOT the top left but rather the bottom left on the grid
         * the initial position of a lawn mower is always clear (no lawn mower is already at that position)
         * it will always be 3 types of letters, R or L for rotation and F for foward movement. No other letters allowed
        */
        for (int row = 1; row < fileContent.Length; row += 2)
        {
            string[] positionAndDirection = fileContent[row].Split(' ');
            int x = int.Parse(positionAndDirection[0]);
            int y = int.Parse(positionAndDirection[1]);
            string orientation = positionAndDirection[2];

            grid[x, y] = "X" + orientation;

            printGrid();

            char[] instructions = fileContent[row + 1].ToCharArray();

            //1st letter is rotation, next letter is movement 
            for (int index = 0; index < instructions.Length; index++)
            {
                char instruction = instructions[index];

                if (instruction == 'L' || instruction == 'R')
                {
                    orientation = switchOrientation(instruction, orientation);
                    grid[x, y] = "X" + orientation;
                    printGrid();
                }
                else //is only movement 'F'
                {
                    //check if moving is possible
                    int[] newXY = getNewCoordinates(x, y, orientation, instruction);
                    if (newXY[0] != x || newXY[1] != y)
                    {
                        grid[x, y] = "0 ";
                        grid[newXY[0], newXY[1]] = "X" + orientation;
                        x = newXY[0];
                        y = newXY[1];
                        printGrid();
                    }
                }
            }
            lawnmowers.Add(lawnmowers.Count, x + " " + y + " " + orientation);
        }
    }
    catch (Exception)
    {
        throw;
    }
}

/*
 * rotation - desired new rotation for lawnmower
 * orientation - current orientation of lawnmower
 * returns new orientation
 */
string switchOrientation(char rotation, string orientation)
{
    string newOrientation;

    switch (orientation)// instructions[index])
    {
        case "N":
            if (rotation.Equals('L'))
            {
                newOrientation = "W";
            }
            else //is R
            {
                newOrientation = "E";
            }
            break;
        case "W":
            if (rotation.Equals('L'))
            {
                newOrientation = "S";
            }
            else //is R
            {
                newOrientation = "N";
            }
            break;
        case "S":
            if (rotation.Equals('L'))
            {
                newOrientation = "E";
            }
            else //is R
            {
                newOrientation = "W";
            }
            break;
        case "E":
            if (rotation.Equals('L'))
            {
                newOrientation = "N";
            }
            else //is R
            {
                newOrientation = "S";
            }
            break;

        default:
            Console.WriteLine("Error: Incorrect value for rotation");
            throw new Exception("Error: Incorrect value for rotation");
            //break; no needed since im throwing on error
    }

    return newOrientation;
}

/*
 * oldX = current x location of lawnmower
 * oldY = current y location of lawnmower
 * orientation = current orientation of lawnmower
 * movement = movement desired for lawnmower
 */
int[] getNewCoordinates(int oldX, int oldY, string orientation, char movement)
{
    int[] newPosition = new int[2] { oldX, oldY };

    switch (orientation)
    {
        case "N":
            if ((oldY + 1 < int.Parse(upperRightCoordinates[1])) && grid[oldX, oldY + 1].Equals("0 "))
            {
                newPosition[0] = oldX;
                newPosition[1] = oldY + 1;
            }
            break;
        case "W":
            if ((oldX - 1 > -1) && grid[oldX - 1, oldY].Equals("0 "))
            {
                newPosition[0] = oldX - 1;
                newPosition[1] = oldY;
            }
            break;
        case "S":
            if ((oldY - 1 > -1) && grid[oldX, oldY - 1].Equals("0 "))
            {
                newPosition[0] = oldX;
                newPosition[1] = oldY - 1;
            } //else not needed , returning old values
            break;
        case "E":
            if ((oldX + 1 < int.Parse(upperRightCoordinates[0])) && grid[oldX + 1, oldY].Equals("0 "))
            {
                newPosition[0] = oldX + 1;
                newPosition[1] = oldY;
            }
            break;
    }


    return newPosition;
}

void displayFinalPositions()
{
    Console.WriteLine("Final Result:");
    List<int> keys = new List<int>(lawnmowers.Keys);
    
    foreach (int key in keys)
    {
        Console.WriteLine(lawnmowers.ElementAt(key).Value); //get.Value);
    }
}
main();