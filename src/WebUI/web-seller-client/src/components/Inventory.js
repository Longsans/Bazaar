import React, { useState, useEffect } from "react";
import CatalogApi from "../api/CatalogApi";
import Auth from "../auth/Auth";
import {
  FulfillmentMethod,
  FulfillmentMethodDisplay,
} from "../constants/FulfillmentMethod.ts";
import { ListingStatus } from "../constants/ListingStatus.ts";
import Radio from "./Radio.js";
import {
  Table,
  TableHeader,
  TableHeaderCell,
  TableBody,
  Button,
} from "semantic-ui-react";
import InventoryItem from "./InventoryItem.js";

export default function Inventory() {
  const [sellerListings, setSellerListings] = useState([]);
  const [filteredListings, setFilteredListings] = useState(sellerListings);
  const [filters, setFilters] = useState([]);
  const [editedItems, setEditedItems] = useState([]);
  const statusFilterName = "statusRadio";
  const fulfillmentFilterName = "fulfillmentRadio";

  const onStatusFilterChange = (event) => {
    const key = "listingStatus";
    const filter = {
      property: key,
      predicate: (listing) =>
        event.target.value === ListingStatus.All ||
        listing.listingStatus === event.target.value,
    };
    let newFilters = filters.filter((x) => x.property != key);
    newFilters.push(filter);
    setFilters(newFilters);
  };

  const onFulfillmentFilterChange = (event) => {
    const key = "fulfillmentMethod";
    const filter = {
      property: key,
      predicate: (listing) =>
        event.target.value === FulfillmentMethod.All ||
        listing.fulfillmentMethod === event.target.value,
    };
    let newFilters = filters.filter((x) => x.property != key);
    newFilters.push(filter);
    setFilters(newFilters);
  };

  const applyFilters = () => {
    let filtered = sellerListings;
    for (const filter of filters) {
      filtered = filtered.filter(filter.predicate);
    }
    setFilteredListings(filtered);
    setEditedItems([]);
  };

  const markItemEdited = (editedItem) => {
    const { productId, price, stock, originalPrice, originalStock } =
      editedItem;
    const item = editedItems.find((x) => x.productId === productId);
    const items = [...editedItems];
    if (item) {
      items[editedItems.indexOf(item)] = editedItem;
    } else items.push(editedItem);
    setEditedItems(items);
  };

  const removeFromEditedItems = (productId) => {
    const items = editedItems.filter((x) => x.productId !== productId);
    setEditedItems(items);
  };

  const saveAllEdits = async () => {
    console.log(editedItems);
    const listingUpdates = editedItems
      .filter((x) => x.price !== x.originalPrice)
      .map((x) => {
        return { productId: x.productId, price: x.price };
      });
    const stockUpdates = editedItems
      .filter((x) => x.stock !== x.originalStock)
      .map((x) => {
        return {
          productId: x.productId,
          units: x.stock,
        };
      });

    try {
      await CatalogApi.bulkUpdateListings(listingUpdates);
    } catch (error) {
      alert(JSON.stringify(error));
    }
    try {
      await CatalogApi.bulkUpdateStock(stockUpdates);
    } catch (error) {
      alert(JSON.stringify(error));
    }
    await pullSellerListings(Auth.getUser().id);
    setEditedItems([]);
  };

  const pullSellerListings = async (sellerId) => {
    try {
      const listings = await CatalogApi.getCatalogByCriteria({
        sellerId: sellerId,
      });
      setSellerListings(listings);
    } catch (error) {
      console.log(error);
    }
  };

  useEffect(() => {
    pullSellerListings(Auth.getUser().id);
  }, []);

  useEffect(() => {
    applyFilters();
  }, [sellerListings]);

  useEffect(() => {
    applyFilters();
  }, [filters]);

  return (
    <div className="m-3">
      <h3>Manage Inventory</h3>
      <div className="d-flex flex-row mt-5">
        <span className="me-5">Filters:</span>
        <span className="me-3">Status:</span>
        <div className="d-flex flex-row" onChange={onStatusFilterChange}>
          <Radio
            name={statusFilterName}
            value={ListingStatus.All}
            displayText="All"
            isDefault
          />
          <span className="me-3" />
          <Radio
            name={statusFilterName}
            value={ListingStatus.Active}
            displayText="Active"
          />
          <span className="me-3" />
          <Radio
            name={statusFilterName}
            value={ListingStatus.InactiveOutOfStock}
            displayText="Out Of Stock"
          />
          <span className="me-3" />
          <Radio
            name={statusFilterName}
            value={ListingStatus.InactiveClosedListing}
            displayText="Listing Closed"
          />
        </div>
        <span className="ms-3 me-3">|</span>
        <span className="me-3">Fulfilled By:</span>
        <div className="d-flex flex-row" onChange={onFulfillmentFilterChange}>
          <Radio
            name={fulfillmentFilterName}
            value={FulfillmentMethod.All}
            displayText={FulfillmentMethodDisplay.All}
            isDefault
          />
          <span className="me-3" />
          <Radio
            name={fulfillmentFilterName}
            value={FulfillmentMethod.Fbb}
            displayText={FulfillmentMethodDisplay.Fbb}
          />
          <span className="me-3" />
          <Radio
            name={fulfillmentFilterName}
            value={FulfillmentMethod.Merchant}
            displayText={FulfillmentMethodDisplay.Merchant}
          />
        </div>
      </div>
      <Table celled={false} className="mt-2" textAlign="left">
        <TableHeader>
          <TableHeaderCell verticalAlign="top">Status</TableHeaderCell>
          <TableHeaderCell verticalAlign="top">Image</TableHeaderCell>
          <TableHeaderCell verticalAlign="top">Product ID</TableHeaderCell>
          <TableHeaderCell verticalAlign="top" className="three wide">
            Product Name
          </TableHeaderCell>
          <TableHeaderCell verticalAlign="top" textAlign="right">
            Available Stock
          </TableHeaderCell>
          {/* <TableHeaderCell verticalAlign="top">Unfulfillable Stock</TableHeaderCell> */}
          <TableHeaderCell verticalAlign="top" textAlign="right">
            Unit Price
          </TableHeaderCell>
          <TableHeaderCell verticalAlign="top">Dimensions</TableHeaderCell>
          <TableHeaderCell verticalAlign="top">Main Department</TableHeaderCell>
          <TableHeaderCell verticalAlign="top">Subcategory</TableHeaderCell>
          <TableHeaderCell verticalAlign="top">Fulfilled By</TableHeaderCell>
          <TableHeaderCell verticalAlign="top" style={{ width: "12.5rem" }}>
            <Button
              color="yellow"
              className="bz-border-radius w-100"
              disabled={!editedItems.length}
              onClick={saveAllEdits}
            >
              Save all
            </Button>
          </TableHeaderCell>
        </TableHeader>
        <TableBody>
          {filteredListings.map((x) => (
            <InventoryItem
              key={x.productId}
              product={x}
              setEditedValues={({
                price,
                stock,
                originalPrice,
                originalStock,
              }) =>
                markItemEdited({
                  productId: x.productId,
                  price,
                  stock,
                  originalPrice,
                  originalStock,
                })
              }
              setItemUnedited={() => removeFromEditedItems(x.productId)}
            />
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
