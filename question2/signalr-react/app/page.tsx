"use client";

import React from "react";
import { useEffect } from "react";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { Button } from "@/components/ui/button";

export default function Home() {

  const [hubConnection, setHubConnection] = React.useState<HubConnection>();
  const [isConnected, setIsConnected] = React.useState<boolean>(false);

  const [usercount, setUserCount] = React.useState<number>(0);
  const [selectedChoice, setSelectedChoice] = React.useState<number>(-1);

  const [pizzaPrice, setPizzaPrice] = React.useState<number>(0);
  const [money, setMoney] = React.useState<number>(0);
  const [nbPizzas, setNbPizzas] = React.useState<number>(0);

  useEffect(() => {
      connectToHub();
    }, []);

  function connectToHub() {
    let newHubConnection = new HubConnectionBuilder()
    .withUrl('http://localhost:5282/hubs/pizza')
    .build();

    // Avoir les méthodes qui recoivent réponses AVANT la connection
    newHubConnection.on("UpdatePizzaPrice", (a:number) => {
      setPizzaPrice(a);
      console.log("Prix de la pizza " + a)
    });

    newHubConnection.on("UpdateMoney", (a:number) => {
      setMoney(a);
      console.log("Nouveau total " + a)
    });

    newHubConnection.on("BuyPizza", (a:number, b:number) => {
      setNbPizzas(a);
      setMoney(b);
      console.log("Nouveau total " + a)
    });

    newHubConnection.on("UpdateMoneyAndNbPizza", (a:number, b:number) => {
      setNbPizzas(a);
      setMoney(b);
    })

    newHubConnection.on("UpdateNbUsers", (a:number) => {
      setUserCount(a);
    })

    setHubConnection(newHubConnection);

    newHubConnection
      .start()
      .then(() =>{ 
        setIsConnected(true)
        hubConnection?.invoke('UpdateNbUsers')
          .catch(err => console.log('Error while UpdateNbUsers' + err));
        console.log("is connected")

        // Si un choix est déjà fait (ex: reconnexion), on l'envoie une fois connecté
        if (selectedChoice >= 0) {
          return newHubConnection.invoke('SelectChoice', selectedChoice);
        }

      })
      .catch(err => console.log('Error while connection' + err));
  }

  function selectChoice(choice:number) {
    setSelectedChoice(choice);
    console.log("Choix de pizza " + (choice))

    // Prévenir le hub du choix (0 = sans ananas, 1 = avec ananas) pour recevoir le prix
    hubConnection?.invoke('SelectChoice', choice)
      .catch(err => console.log('Error while SelectChoice' + err));
    hubConnection?.invoke('UpdateMoneyAndNbPizza', choice)
      .catch(err => console.log('Error while UpdateMoneyAndNbPizza' + err));
    hubConnection?.invoke('UpdateNbUsers')
      .catch(err => console.log('Error while UpdateNbUsers' + err));

  }

  function unselectChoice() {
    // Le hub attend le choix actuel (on l'envoie avant de réinitialiser)
    hubConnection?.invoke('UnselectChoice', selectedChoice)
      .catch(err => console.log('Error while UnselectChoice' + err));

    setSelectedChoice(-1);
  }

  function addMoney() {
    hubConnection?.invoke("UpdateMoney", selectedChoice)
    .catch(err => console.log('Error while addMoney' + err));
  }

  function buyPizza() {
    hubConnection?.invoke("BuyPizza", selectedChoice)
    .catch(err => console.log('Error while buypizza' + err));
  }

  return (
    <>
      <img src="pizzaHub.png" style={{ height: 160 }} />
      <div className="main">
        {!isConnected && (
          <div>
            Connecting...
          </div>
        )}
        {isConnected && (
          <div className="m-4">
            <h1>Achat de pizza en groupe!</h1>
            <div>
              Nb connected users: {usercount}
            </div>
            <br></br>
            {selectedChoice < 0 && (
              <>
                <h2>Choisissez une pizza</h2>
                <div className="flex gap-4 w-full max-w-4xl mx-auto gap-4">

                  <img onClick={() => selectChoice(0)} src="pizza.png" style={{ height: 195 }} />
                  <img onClick={() => selectChoice(1)} src="pizzaAnanas.png" style={{ height: 195 }} />
                </div>
              </>
            )}
            {selectedChoice >= 0 && (
              <div>
                <h2>Votre pizza</h2>
                {selectedChoice === 0 && (
                  <img src="pizza.png" style={{ height: 195 }} />
                )}
                {selectedChoice === 1 && (
                  <img src="pizzaAnanas.png" style={{ height: 195 }} />
                )}
                <div>
                  <Button onClick={unselectChoice}>Changer de pizza</Button>
                </div>
                <h2>Achetez des pizzas</h2>
                <div className="section">
                  Prix d'une pizza: <b>{pizzaPrice}$</b>
                </div>
                <div className="section">
                  <span className="info">Total d'argent: {money}$</span>
                  <Button onClick={addMoney}>Ajouter 2$</Button>
                </div>
                <div className="section">
                  <span className="info">Nombre de pizzas: {nbPizzas}</span>
                  <Button disabled={money < pizzaPrice} onClick={buyPizza}>Acheter une pizza</Button>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </>
  );
}