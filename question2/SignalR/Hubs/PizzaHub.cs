using Microsoft.AspNetCore.Connections;
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

        public async Task UpdateNbUsers()
        {
            var nbreuser = _pizzaManager.NbConnectedUsers;
            await Clients.All.SendAsync("UpdateNbUsers", nbreuser);
        }

        public async Task SelectChoice(PizzaChoice choice)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, choice.ToString()+ "_Group");
            UpdateNbPizzasAndMoney(choice);
            UpdatePizzaPrice(choice);

        }

        public async Task UnselectChoice(PizzaChoice choice)
        {
            Groups.RemoveFromGroupAsync(Context.ConnectionId, choice.ToString());
        }

        public async Task AddMoney(PizzaChoice choice)
        {
            _pizzaManager.IncreaseMoney(choice);
            UpdateMoney(choice);
        }

        public async Task BuyPizza(PizzaChoice choice)
        {

            _pizzaManager.BuyPizza(choice);
            UpdateNbPizzasAndMoney(choice);
        }

        public async Task UpdateMoney(PizzaChoice choice)
        {
            var moneypool = _pizzaManager.GetMoney(choice);
            var groupname = _pizzaManager.GetGroupName(choice);

            await Clients.Group(groupname).SendAsync("UpdateMoney", moneypool);
        }

        public async Task UpdateNbPizzasAndMoney(PizzaChoice choice)
        {
            var moneypool = _pizzaManager.GetMoney(choice);
            var nbPizza = _pizzaManager.GetNbPizzas(choice);
            var groupname = _pizzaManager.GetGroupName(choice);

            await Clients.Group(groupname).SendAsync("UpdateNbPizzasAndMoney", nbPizza, moneypool);
        }

        public async Task UpdatePizzaPrice(PizzaChoice choice)
        {
            var pizzaprice = _pizzaManager.GetPizzaPrice(choice);
            var groupname = _pizzaManager.GetGroupName(choice);

            await Clients.Group(groupname).SendAsync("UpdatePizzaPrice", pizzaprice);
        }
    }
}
