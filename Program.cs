using App.Controllers;

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = HomeController.Execute();
}