using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR.Services;

namespace SignalR.Hubs
{
    public class PizzaHub : Hub
    {
        private readonly PizzaManager _pizzaManager;

        public PizzaHub(PizzaManager pizzaManager) {
            _pizzaManager = pizzaManager;
        }

        public override async Task OnConnectedAsync()
        {
            _pizzaManager.AddUser();
            await base.OnConnectedAsync();
            UpdateNbUsers();
            
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _pizzaManager.RemoveUser();
            await base.OnConnectedAsync();
            UpdateNbUsers();
        }

        public async Task SelectChoice(PizzaChoice choice)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, choice.ToString() + "_Group");
            var price = _pizzaManager.GetPizzaPrice(choice);
            await Clients.Caller.SendAsync("UpdatePizzaPrice", price);
            
        }

        public async Task UnselectChoice(PizzaChoice choice)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, choice.ToString() + "_Group");
        }

        public async Task UpdateNbUsers()
        {
            var nbreUser = _pizzaManager.NbConnectedUsers;
            await Clients.All.SendAsync("UpdateNbUsers", nbreUser);
        }

        public async Task UpdateMoney(PizzaChoice choice)
        {
           _pizzaManager.IncreaseMoney(choice);
            var groupname = _pizzaManager.GetGroupName(choice);
            var moneyPool = _pizzaManager.GetMoney(choice);
            await Clients.Group(groupname).SendAsync("UpdateMoney", moneyPool);
        }

        public async Task BuyPizza(PizzaChoice choice)
        {
            var groupname = _pizzaManager.GetGroupName(choice);
            _pizzaManager.BuyPizza(choice);
            var nbrePizza = _pizzaManager.GetNbPizzas(choice);
            var moneyPool = _pizzaManager.GetMoney(choice);
            await Clients.Group(groupname).SendAsync("BuyPizza", nbrePizza, moneyPool);

        }

        public async Task UpdatePizza (PizzaChoice choice)
        {
            var groupname = _pizzaManager.GetGroupName(choice);

            var nbrePizza = _pizzaManager.GetNbPizzas(choice);

            await Clients.Group(groupname).SendAsync("UpdatePizza", nbrePizza);
        }

        public async Task UpdateMoneyAndNbPizza(PizzaChoice choice)
        {
            var groupname = _pizzaManager.GetGroupName(choice);

            var nbrePizza = _pizzaManager.GetNbPizzas(choice);

            var moneyPool = _pizzaManager.GetMoney(choice);

            await Clients.Group(groupname).SendAsync("UpdateMoneyAndNbPizza", nbrePizza, moneyPool);
        }
    }
}
