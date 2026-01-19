using App.Controller;
using App.View;

HomeMenu homeMenu = new();
HomeController homeController = new(homeMenu);

bool homeLoop = true;

while (homeLoop)
{
    homeLoop = homeController.Execute();
}