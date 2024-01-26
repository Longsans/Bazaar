import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import CatalogApi from "../api/CatalogApi";
import SellerApi from "../api/SellerApi";
import "../site.scss";
import { useBasket } from "../hooks/useBasket";
import { FulfillmentMethod } from "../constants/FulfillmentMethod.ts";

export function ProductDetails() {
  const { productId } = useParams();
  const [product, setProduct] = useState();
  const [loading, setLoading] = useState(false);
  const [quantity, setQuantity] = useState(1);
  const { basket, addItemToBasket, changeItemQuantity } = useBasket();
  const [distributor, setDistributor] = useState();
  const [seller, setSeller] = useState();

  const handleAddToBasket = async () => {
    setLoading(true);
    const basketItem = basket.items.find((x) => x.productId === productId);
    try {
      if (basketItem) {
        await changeItemQuantity(productId, +basketItem.quantity + +quantity);
      } else {
        await addItemToBasket(productId, +quantity);
      }
      alert("Items added to basket.");
    } catch (error) {
      alert(error);
    }
    setLoading(false);
  };

  const formatSellerName = (seller) => `${seller.firstName} ${seller.lastName}`;

  useEffect(() => {
    const fetchProductAndSeller = async () => {
      setLoading(true);
      const productData = await CatalogApi.getProductByProductId(productId);
      const sellerData = await SellerApi.getSellerById(productData.sellerId);
      setProduct(productData);
      setSeller(sellerData);
      setDistributor(
        productData.fulfillmentMethod == FulfillmentMethod.Merchant
          ? formatSellerName(sellerData)
          : "Bazaar"
      );
      setLoading(false);
    };
    fetchProductAndSeller();
  }, []);

  const sideInfoItem = (itemName, value) => (
    <tr>
      <td className="pe-3">
        <small className="text-secondary semi-bold">{itemName}</small>
      </td>
      <td className="ms-2">
        <small>{value}</small>
      </td>
    </tr>
  );

  return loading ? (
    <h5>Loading...</h5>
  ) : product ? (
    <div className="row pt-4">
      <img
        className="col"
        src={product.imageUrl}
        style={{ maxWidth: "35%", maxHeight: "35%" }}
      />
      <div className="col ps-4">
        <h3>
          <span className="semi-bold">{product.productName}</span>
        </h3>
        <hr />
        <h3>
          <span className="semi-bold">${product.price}</span>
        </h3>
        <p>{product.productDescription}</p>
        <h5>In stock: {product.availableStock}</h5>

        <div>
          <h5 className="my-3">Quantity:</h5>
          <select
            className="form-select w-25"
            id="quantitySelect"
            onChange={(event) => setQuantity(event.target.value)}
            style={{ cursor: "pointer", marginBottom: ".75rem" }}
          >
            <option value={1}>1</option>
            <option value={2}>2</option>
            <option value={3}>3</option>
            <option value={4}>4</option>
            <option value={5}>5</option>
            <option value={6}>6</option>
            <option value={7}>7</option>
            <option value={8}>8</option>
            <option value={9}>9</option>
            <option value={10}>10</option>
          </select>
          <button
            className="btn baz-btn-primary p-2 w-25"
            onClick={handleAddToBasket}
          >
            <h5 className="m-0">Add to basket</h5>
          </button>
          <table className="mt-3">
            {sideInfoItem("Ships from", distributor)}
            {sideInfoItem("Sold by", formatSellerName(seller))}
            {sideInfoItem("Customer service", distributor)}
          </table>
        </div>
      </div>
    </div>
  ) : (
    <div>
      <h4>Sorry, we weren't able to find that product.</h4>
      <p>
        Either the product has been removed from Bazaar by the seller or its
        listing is currently not open. You can try again later if it's the
        latter case.
      </p>
    </div>
  );
}
