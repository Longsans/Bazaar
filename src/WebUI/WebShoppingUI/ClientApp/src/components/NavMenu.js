import React, { useEffect, useState } from "react";
import {
  Collapse,
  NavbarToggler,
  Navbar,
  NavbarBrand,
  NavItem,
  NavLink,
} from "reactstrap";
import { Link } from "react-router-dom";
import "./NavMenu.css";

export function NavMenu() {
  // const [logoutUrl, setLogoutUrl] = useState();
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
    if (process.env.NODE_ENV === "development") return;

    setTimeout(() => {
      window.location.reload();
    }, 500);
  }

  // useEffect(() => {
  //   const getSession = async () => {
  //     var req = new Request(Bff.userPath, {
  //       headers: new Headers({
  //         "X-CSRF": "1",
  //       }),
  //     });

  //     var resp = await fetch(req);
  //     if (resp.ok) {
  //       var claims = await resp.json();
  //       let logoutPath = Bff.logoutPath;
  //       if (claims) {
  //         logoutPath = claims.find(
  //           (claim) => claim.type === Bff.logoutPathClaimName
  //         ).value;
  //       }
  //       setLogoutUrl(getBffUri(logoutPath));
  //     } else {
  //       throw Error(
  //         `Session responded with error: ${resp.status} ${resp.statusText}`
  //       );
  //     }
  //   };
  //   getSession();
  // }, []);

  return (
    <header>
      <Navbar
        className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3"
        container
        light
      >
        <NavbarBrand tag={Link} to="/">
          Bazaar
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
            <>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/catalog">
                  Catalog
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/basket">
                  Basket
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/orders">
                  Order history
                </NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to="/profile">
                  Profile
                </NavLink>
              </NavItem>
            </>
            {/* {!logoutUrl && (
              <NavItem>
                <NavLink
                  tag={Link}
                  className="text-dark"
                  to={getBffUri(Bff.loginPath)}
                  onClick={refreshPage}
                >
                  Login
                </NavLink>
              </NavItem>
            )} */}
            {/* {logoutUrl && (
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
            )} */}
          </ul>
        </Collapse>
      </Navbar>
    </header>
  );
}
