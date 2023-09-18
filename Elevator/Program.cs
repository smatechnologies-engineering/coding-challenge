// See https://aka.ms/new-console-template for more information
bool IsOn = true;
while (IsOn)
{
  Console.WriteLine("Hello, World, enter a floor or q to quit");
  string floor = Console.ReadLine();
  Console.WriteLine($"you want floor {floor}");

  if (floor == "q")
  {
    IsOn = false;
    Console.WriteLine("turn elevator off");
  }
}

