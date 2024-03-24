import React, { useEffect, useState } from "react";
import {
  TableRow,
  TableCell,
  Button,
  Dropdown,
  DropdownMenu,
  DropdownItem,
} from "semantic-ui-react";
import {
  FulfillmentMethod,
  FulfillmentMethodDisplay,
} from "../constants/FulfillmentMethod.ts";
import { Label, Input } from "semantic-ui-react";
import { NumericFormat } from "react-number-format";
import "../site.scss";

export default function InventoryItem({
  product,
  setEditedValues,
  setItemUnedited,
}) {
  const [edited, setEdited] = useState(false);
  const [price, setPrice] = useState(product.price);
  const [stock, setStock] = useState(product.availableStock);
  const actions = {
    Edit: () => {},
    [product.fulfillmentMethod === FulfillmentMethod.Fbb
      ? "Change to Fulfilled by Merchant"
      : "Change to Fulfilled by Bazaar"]: () => {},
    "Send/Replenish inventory": () => {},
    "Create removal order": () => {},
    "Close Listing": () => {},
    "Delete product and listing": () => {},
  };
  const priceIsValid = price > 0;
  const stockIsValid = stock >= 0;

  const handlePriceEdited = ({ floatValue }) => {
    if (!edited) setEdited(true);
    setPrice(floatValue);
  };

  const handleStockEdited = ({ floatValue }) => {
    if (!edited) setEdited(true);
    setStock(floatValue);
  };

  const handlePriceInputLostFocus = () => {
    if (!priceIsValid) {
      setPrice(product.price);
    }
  };

  const handleStockInputLostFocus = () => {
    if (!stockIsValid) {
      setStock(product.availableStock);
    }
  };

  useEffect(() => {
    if (edited && priceIsValid && stockIsValid) {
      setEditedValues({
        price,
        stock,
        originalPrice: product.price,
        originalStock: product.availableStock,
      });
    } else setItemUnedited();
  }, [edited, price, stock]);

  return (
    <TableRow key={product.productId}>
      <TableCell>{product.listingStatus}</TableCell>
      <TableCell>
        <img src={product.imageUrl} style={{ maxHeight: "80px" }} />
      </TableCell>
      <TableCell>{product.productId}</TableCell>
      <TableCell>{product.productName}</TableCell>
      <TableCell textAlign="right">
        {product.fulfillmentMethod === FulfillmentMethod.Merchant ? (
          <div className="d-flex flex-row-reverse">
            <Input type="text">
              <NumericFormat
                className="text-end"
                style={{ width: "6.5rem" }}
                value={stock}
                allowNegative={false}
                allowLeadingZeros={false}
                decimalScale={0}
                thousandsGroupStyle="thousand"
                thousandSeparator=","
                onValueChange={handleStockEdited}
                onBlur={handleStockInputLostFocus}
              />
            </Input>
          </div>
        ) : (
          <span className="pe-3">{product.availableStock}</span>
        )}
      </TableCell>
      <TableCell>
        <div className="d-flex flex-row-reverse">
          <Input type="text" labelPosition="left">
            <Label basic>
              <span className="text-secondary">$</span>
            </Label>
            <NumericFormat
              className="text-end"
              style={{ width: "7rem" }}
              value={price}
              allowNegative={false}
              allowLeadingZeros={false}
              thousandsGroupStyle="thousand"
              thousandSeparator=","
              decimalScale={2}
              fixedDecimalScale
              onValueChange={handlePriceEdited}
              onBlur={handlePriceInputLostFocus}
            />
          </Input>
        </div>
      </TableCell>
      {/* unfulfillable stock-- <TableCell>{x.fulfillmentMethod === FulfillmentMethod.Fbb ? ...}</TableCell> */}
      <TableCell>{`${product.dimensions.lengthInCm}cm x ${product.dimensions.widthInCm}cm x ${product.dimensions.heightInCm}cm`}</TableCell>
      <TableCell>{product.mainDepartment.name}</TableCell>
      <TableCell>{product.subcategory.name}</TableCell>
      <TableCell>
        {FulfillmentMethodDisplay[product.fulfillmentMethod]}
      </TableCell>
      <TableCell>
        <Button as="div" labelPosition="right" className="d-flex flex-row">
          <Button
            className="fw-normal text-dark flex-grow-1 text-start bz-i-control"
            color="google"
          >
            Edit
          </Button>
          <Label className="p-0 bz-i-control">
            <Dropdown
              compact
              scrolling={false}
              className="w-100 h-100 pt-2 pb-2 pe-3"
              direction="left"
            >
              <DropdownMenu>
                {Object.entries(actions).map((x, index) => {
                  return (
                    <DropdownItem key={index} text={x[0]} onClick={x[1]} />
                  );
                })}
              </DropdownMenu>
            </Dropdown>
          </Label>
        </Button>
      </TableCell>
    </TableRow>
  );
}
