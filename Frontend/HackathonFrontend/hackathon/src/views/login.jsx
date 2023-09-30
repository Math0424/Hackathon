import { useState } from "react";
import {useNavigate} from 'react-router-dom';

const loginValidates = (username, password) => {
    return true;
}

const Login = () => {
    const [username, setusername] = useState("");
    const [password, setpassword] = useState("");
    const navigate = useNavigate();
    const handleSubmit = (e) => {
        if(loginValidates(username, password)) {
            navigate('/Cow');
        }
    };
    return (
        <div>
            <h1>Please log in</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    name="Username"
                    value={username}
                    onChange={(e) => setusername(e.target.value)}
                />
                <input
                    type="password"
                    name="Password"
                    onChange={(e) => setpassword(e.target.value)}
                />
                <input type="submit" value="Submit" />
            </form>
        </div>
    )
}

export default Login;