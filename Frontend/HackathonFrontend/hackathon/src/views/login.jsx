import { useState } from "react";
import {useNavigate} from 'react-router-dom';
import Cookies from 'js-cookie';

const loginValidates = async (user, pass) => {
    const response = await fetch('http://localhost:5000/user/auth', {
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
}
const createAccount = async (user, pass) => {
    const response = await fetch('http://localhost:5000/user/create', {
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
        console.log(response);
    return response;
}

const Login = () => {
    const [username, setusername] = useState("");
    const [password, setpassword] = useState("");
    const navigate = useNavigate();
    const handleSubmit = async () => {
        const response = await loginValidates(username, password)

        if (response[0] !== undefined) {
            Cookies.set('userId', response[0])

            navigate('/cowview');
        }
    };
     const handleSubmit2 = async () => {
            const response1 = await createAccount(username, password)
            console.log(response1);
            const response = await loginValidates(username, password)
            if (response[0] !== undefined) {
                Cookies.set('userId', response[0])
                navigate('/cowview');
            }
        };

    return (
        <div>
            <h2>Cow Smart</h2>
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
            <input type="button" onClick={handleSubmit2} value="Create Account" />
        </div>
    )
}

export default Login;