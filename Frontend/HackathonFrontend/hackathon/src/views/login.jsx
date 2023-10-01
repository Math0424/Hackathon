import { useState } from "react";
import {useNavigate} from 'react-router-dom';
import Cookies from 'js-cookie';

const loginValidates = async (user, pass) => {
    const response = await fetch('http://localhost:5000/user/auth', {
        method: "POST", // *GET, POST, PUT, DELETE, etc.
        mode: "cors", // no-cors, *cors, same-origin
        cache: "no-cache", // *default, no-cache, reload, force-cache, only-if-cached
        credentials: "same-origin", // include, *same-origin, omit
        headers: {
            "Content-Type": "application/json",
            // 'Content-Type': 'application/x-www-form-urlencoded',
        },
        redirect: "follow", // manual, *follow, error
        referrerPolicy: "no-referrer", // no-referrer, *no-referrer-when-downgrade, origin, origin-when-cross-origin, same-origin, strict-origin, strict-origin-when-cross-origin, unsafe-url
        body: JSON.stringify({
            username: user,
            password: pass
        }), // body data type must match "Content-Type" header
    });

    console.log(response.json());
    return response.json(); // parses JSON response into native JavaScript objects
}

const Login = () => {
    const [username, setusername] = useState("");
    const [password, setpassword] = useState("");
    const navigate = useNavigate();
    const handleSubmit = async () => {
        const response = await loginValidates(username, password)
        if (response.id !== undefined) {
            Cookies.set('userId', response.id)

            navigate('/Cow');
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
        </div>
    )
}

export default Login;