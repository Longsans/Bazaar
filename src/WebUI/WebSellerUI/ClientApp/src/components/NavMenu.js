import React, { useEffect, useState } from "react";
import {
  Collapse,
  Navbar,
  NavbarBrand,
  NavbarToggler,
  NavItem,
  NavLink,
} from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";

export function NavMenu() {
  const [logoutUrl, setLogoutUrl] = useState();
  const [navbarCollapsed, setNavbarCollapsed] = useState(true);

  function toggleNavbar() {
    setNavbarCollapsed(!navbarCollapsed);
  }

  function getBffUri(path) {
    return process.env.NODE_ENV === "development"
      ? `http://localhost:${process.env.REACT_APP_BFF_PORT}${path}`
      : path;
  }

  function refreshPage() {
    setTimeout(
      () => {
        window.location.reload();
      },
      process.env.NODE_ENV === "development" ? 1500 : 500
    );
  }

  useEffect(() => {
    const getSession = async () => {
      var req = new Request("/bff/user", {
        headers: new Headers({
          "X-CSRF": "1",
        }),
      });

      var resp = await fetch(req);
      if (resp.ok) {
        var claims = await resp.json();
        let logoutPath = "/bff/logout";
        if (claims) {
          logoutPath = claims.find(
            (claim) => claim.type === "bff:logout_url"
          ).value;
        }
        setLogoutUrl(getBffUri(logoutPath));
      } else {
        throw Error(
          `Session responded with error: ${resp.status} ${resp.statusText}`
        );
      }
    };
    getSession();
  }, []);

  return (
    <header>
      <Navbar
        className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3"
        container
        light
      >
        <NavbarBrand tag={Link} to="/">
          WebSellerUI
        </NavbarBrand>
        <NavbarToggler onClick={toggleNavbar} className="mr-2" />
        <Collapse
          className="d-sm-inline-flex flex-sm-row-reverse"
          isOpen={!navbarCollapsed}
          navbar
        >
          <ul className="navbar-nav flex-grow">
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/">
                Home
              </NavLink>
            </NavItem>
            {logoutUrl && (
              <>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/catalog">
                    Catalog
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/orders">
                    Orders
                  </NavLink>
                </NavItem>
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/contracts">
                    Contracts
                  </NavLink>
                </NavItem>
              </>
            )}
            {!logoutUrl && (
              <NavItem>
                <NavLink
                  tag={Link}
                  className="text-dark"
                  to={getBffUri(`/bff/login`)}
                  onClick={refreshPage}
                >
                  Login
                </NavLink>
              </NavItem>
            )}
            {logoutUrl && (
              <NavItem>
                <NavLink
                  tag={Link}
                  className="text-dark"
                  to={logoutUrl}
                  onClick={refreshPage}
                >
                  Logout
                </NavLink>
              </NavItem>
            )}
          </ul>
        </Collapse>
      </Navbar>
    </header>
  );
}
