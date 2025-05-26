import React from 'react';
import {Container , Row, Col, Card, Button} from 'react-bootstrap';
import {getToken} from '../../utils/tokens_management';

const HomePages = () =>{

    //Hiển thị cookie để kiểm tra
    const cookies = document.cookie;
    console.log(cookies);

    getToken("access_token");
    getToken("refresh_token");

    return(
        <div>
            <div className='bg-primaey text-white p-5 mb-4'>
                <h1>Welcome to E-Commerce Shop</h1>
                <p className="lead">Discover our amazing products with great deals</p>
                <Button variant="light">Shop Now</Button>
            </div>
        </div>
    );
};

export default HomePages;