import React from 'react';
import {Navbar, Container, Nav, Button} from 'react-bootstrap';
import {Link, useNavigate} from 'react-router-dom';
import {Logout} from '../../services/authenServices';
import { isAuthenticated } from '../../utils/tokens_management';

const Header = () =>{
    const navigate = useNavigate();
    const authenticated = isAuthenticated();

    //Xử lý đăng xuất 
    const handleLogout = async () =>{
        try{
            await Logout();
            navigate('/login');
        }
        catch(error){
            console.error("Error during logout:", error);
        }
    };

    return (
        <Navbar bg='dark' variant='dark' expand='lg'>
            <Container>
                <Navbar.Brand as={Link} to="/">E-Commerce Shop</Navbar.Brand>
                <Navbar.Toggle aria-controls='basic-navbar-nav'/>
                <Navbar.Collapse id='baasic-navbar-nav'>
                    <Nav className="me-auto">
                        <Nav.Link as={Link} to="/">Home</Nav.Link>
                        <Nav.Link as={Link} to="/products">Products</Nav.Link>
                    </Nav>
                    <Nav>
                        {authenticated ? (
                            <Button variant='outline-light' onClick={handleLogout}>Đăng xuất</Button>
                        ):(
                            <Nav.Link as={Link} to="/login">Đăng nhập</Nav.Link>
                        )} 
                    </Nav>
                </Navbar.Collapse>
            </Container>
        </Navbar>
    )
}

export default Header;