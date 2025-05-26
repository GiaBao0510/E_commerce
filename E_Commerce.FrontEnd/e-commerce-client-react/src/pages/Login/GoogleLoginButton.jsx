import React, {useState} from "react";
import {Button} from 'react-bootstrap';

const GoogleLoginButton = () =>{
    
    const [isLoading, setIsLoading] = useState(false);

    const handleGoogleLogin = () =>{

        setIsLoading(true);

        //Chuyển hướng đến endpoint của backend để bắt đầu quá trình đăng nhập
        const returnUrl = encodeURIComponent(window.location.origin + '/auth/callback');
        
        const googleAththUrl = `${process.env.REACT_APP_BASE_URL}/auth/signin-google?returnUrl=` + returnUrl;
        console.log(googleAththUrl);
        window.location.href = googleAththUrl;
    };

    return (
        <Button 
            onClick={handleGoogleLogin} 
            variant="light"
            className="d-flex align-items-center justify-content-center gap-2 w-100 mb-3"
            disabled={isLoading}
        >
            {isLoading ? 
                (
                    <>
                        <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                        <span>Đang chuyển hướng ....</span>
                    </>
                ):(
                    <>
                        <img 
                            src="https://developers.google.com/identity/images/g-logo.png" 
                            alt="Google logo" 
                            style={{width: '20px', height: '20px'}} 
                        />
                        <span>Sign in with Google</span>
                    </> 
                )
            }
            
        </Button>
    );
};

export default GoogleLoginButton;
