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
  TableRow,
  TableCell,
  Input,
  Label,
} from "semantic-ui-react";

export default function Inventory() {
  const [sellerListings, setSellerListings] = useState([]);
  const [filteredListings, setFilteredListings] = useState(sellerListings);
  const [filters, setFilters] = useState([]);
  const statusFilterName = "statusRadio";
  const fulfillmentFilterName = "fulfillmentRadio";

  const onStatusFilterChange = (event) => {
    console.log(event.target.value);
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
  };

  useEffect(() => {
    const fetchSellerListings = async (sellerId) => {
      try {
        const listings = await CatalogApi.getCatalogByCriteria({
          sellerId: sellerId,
        });
        setSellerListings(listings);
      } catch (error) {
        console.log(error);
      }
    };
    fetchSellerListings(Auth.getUser().id);
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
      <Table celled={false} className="mt-2 text-start">
        <TableHeader>
          <TableHeaderCell>Status</TableHeaderCell>
          <TableHeaderCell>Image</TableHeaderCell>
          <TableHeaderCell>Product ID</TableHeaderCell>
          <TableHeaderCell>Product Name</TableHeaderCell>
          <TableHeaderCell textAlign="right">Available Stock</TableHeaderCell>
          {/* <TableHeaderCell>Unfulfillable Stock</TableHeaderCell> */}
          <TableHeaderCell textAlign="right">Unit Price</TableHeaderCell>
          <TableHeaderCell>Dimensions</TableHeaderCell>
          <TableHeaderCell>Main Department</TableHeaderCell>
          <TableHeaderCell>Subcategory</TableHeaderCell>
          <TableHeaderCell>Fulfilled By</TableHeaderCell>
        </TableHeader>
        <TableBody>
          {filteredListings.map((x) => (
            <TableRow key={x.productId}>
              <TableCell>{x.listingStatus}</TableCell>
              <TableCell>
                <img src={x.imageUrl} style={{ maxHeight: "80px" }} />
              </TableCell>
              <Table.Cell>{x.productId}</Table.Cell>
              <TableCell>{x.productName}</TableCell>
              <TableCell textAlign="right">
                {x.fulfillmentMethod === FulfillmentMethod.Merchant ? (
                  <div className="d-flex flex-row-reverse">
                    <Input value={x.availableStock} type="text">
                      <input className="text-end" style={{ width: "100px" }} />
                    </Input>
                  </div>
                ) : (
                  <span className="pe-3">{x.availableStock}</span>
                )}
              </TableCell>
              <Table.Cell>
                <div className="d-flex flex-row-reverse">
                  <Input value={x.price} type="text" labelPosition="left">
                    <Label basic>
                      <span className="text-secondary">$</span>
                    </Label>
                    <input className="text-end" style={{ width: "100px" }} />
                  </Input>
                </div>
              </Table.Cell>
              {/* unfulfillable stock-- <TableCell>{x.fulfillmentMethod === FulfillmentMethod.Fbb ? ...}</TableCell> */}
              <TableCell>{`${x.dimensions.lengthInCm}cm x ${x.dimensions.widthInCm}cm x ${x.dimensions.heightInCm}cm`}</TableCell>
              <TableCell>{x.mainDepartment.name}</TableCell>
              <TableCell>{x.subcategory.name}</TableCell>
              <TableCell>
                {FulfillmentMethodDisplay[x.fulfillmentMethod]}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
