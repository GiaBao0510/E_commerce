import React, {useState} from "react";
import { Card, Form, Button, Alert } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import {Login} from "../../services/authenServices";
import LoadingSpinner from "../../components/Ui/LoadingSpinner";
import GoogleLoginButton from "./GoogleLoginButton";

const LoginPage = () =>{
    const [phoneNumer, setPhoneNumber] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const navigate = useNavigate();

    //Xử lý sự kiện khi đăng nhập
    const handleLogin = async (e) =>{
        
        e.preventDefault(); // Ngăn chặn hành vi mặc định của form

        if(!phoneNumer || !password){
            setError("Vui lòng nhập số điện thoại và mật khẩu");
            return;
        }

        setLoading(true);
        setError("");

        try {
            await Login(phoneNumer, password);
            
            //Nếu đăng nhập thành công thì chuyển hướng home
            navigate("/home");
        } catch (error) {
            console.error("Đăng nhập thất bại:", error);
            setError(error.message || "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin đăng nhập.");
        }
        finally{
            setLoading(false);
        }
    };

    return (
        <div className="d-flex justify-content-center align-items-center pt-5">
            <Card className="shadow" style={{maxWidth: "450px", width:"100%"}}>
                <Card.Header className="bg-primary text-white text-center py-3">
                    <h2>Đăng Nhập</h2>
                </Card.Header>
                <Card.Body className="p-4">

                    {error && <Alert variant="danger"> {error}</Alert>}

                    <Form onSubmit={handleLogin}>
                        <Form.Group className="mb-3">
                            <Form.Label>Số điện thoại</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="Nhập số điện thoại"
                                value={phoneNumer}
                                onChange={(e) => setPhoneNumber(e.target.value)}
                                disabled={loading}
                            />
                        </Form.Group>

                        <Form.Group className="mb-3">
                            <Form.Label>Mật khẩu</Form.Label>
                            <Form.Control
                                type="password"
                                placeholder="Nhập mật khẩu"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                disabled={loading}
                            />
                        </Form.Group>

                        <div className="d-grid gap-2">
                            <Button variant="primary" type="submit" disabled={loading}>
                                {loading ? 'Đang đăng nhập...': 'Đăng nhập'}
                            </Button>
                        </div>

                        <div className="text-center mb-4">
                            <div className="position-relative">
                                <hr className="my-2" />
                                <span className="position-absolute top-50 start-50 translate-middle px-3 bg-white text-muted">
                                    hoặc
                                </span>
                            </div>
                        </div>

                        <div className="mb-4">
                            <GoogleLoginButton/>
                            {/*Có thể đăng nhập thông qua nền tảng khác như sau*/}
                        </div>
                    </Form>

                    {loading && <LoadingSpinner/>}
                </Card.Body>
            </Card>
        </div>
    );
};

export default LoginPage;