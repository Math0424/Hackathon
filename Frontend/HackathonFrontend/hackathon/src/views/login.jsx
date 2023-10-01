import { useState } from "react";
import {useNavigate} from 'react-router-dom';
import Cookies from 'js-cookie';

const loginValidates = async (user, pass) => {
    try {
        const response = await fetch('http://173.215.25.174:5000/user/auth', {
            method: "POST",
            mode: "cors",
            cache: "no-cache",
            headers: {
                "Content-Type": "application/json",
            },
            redirect: "follow",
            referrerPolicy: "no-referrer",
            body: JSON.stringify({
                username: user,
                password: pass
            })
        });

        return response.json();
    } catch (e) {
        return [undefined];
    }
}

const Login = () => {
    const [username, setusername] = useState("");
    const [password, setpassword] = useState("");
    const [isErrorLoggingIn, setIsLogInError] = useState(false);
    const navigate = useNavigate();
    const handleSubmit = async () => {
        const response = await loginValidates(username, password)

        if (response[0] !== undefined) {
            Cookies.set('userId', response[0])

            navigate('/Cow');
        } else {
            setIsLogInError(true);
        }
    };

    return (
        <div>
            <h1>Please log in</h1>
            <input
                onChange={(e) => setusername(e.target.value)}
                placeholder={'Username'}
                name="Username"
                type="text"
                value={username}
            />
            <input
                name="Password"
                onChange={(e) => setpassword(e.target.value)}
                placeholder={'Password'}
                type="password"
            />
            <input type="submit" onClick={handleSubmit} value="Submit" />
            <input type="button" value="Create Account" />
            {isErrorLoggingIn && <p>Error logging in</p>}
        </div>
    )
}

export default Login;