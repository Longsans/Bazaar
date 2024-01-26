import React, { Component } from "react";
import { Link } from "react-router-dom";

export class Home extends Component {
  static displayName = Home.name;

  render() {
    return (
      <div>
        <h1>Welcome to Bazaar!</h1>
        <p>
          Bazaar is the largest market in the world, where you can find anything
          you need and purchase it from any seller of your choosing.
        </p>
        <ul>
          <li>
            Browse our catalog and add products you like to basket via our{" "}
            <Link to="/catalog">Catalog</Link> page.
          </li>
          <li>
            See products you've added to basket and proceed to checkout via{" "}
            <Link to="/basket">Basket</Link> page.
          </li>
          <li>
            See your order history and cancel orders you no longer want on{" "}
            <Link to="/orders">Order history</Link> page.
          </li>
          <li>
            Your profile and personal information can be changed on the{" "}
            <Link to="/profile">Profile</Link> page.
          </li>
        </ul>
        <p>
          Bazaar does not charge extra fee for its service, nor will it ever do
          so.
        </p>
      </div>
    );
  }
}
