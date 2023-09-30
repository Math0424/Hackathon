import { useState } from "react";

const loginValidates = (username, password) => {

}

const Cow = () => {
    const [username, setusername] = useState("");
    const [password, setpassword] = useState("");
    const handleSubmit = (e) => {
        if(loginValidates(username, password)) {

        }
    };
    return (
        <div>
            <h1>Cow</h1>
            <form onSubmit={handleSubmit}>
                <input
                    type="text"
                    name="Username"
                    value={username}

                />
                <input
                    type="password"
                    name="Password"

                />
                <input type="submit" value="Submit" />
            </form>
        </div>
    )
}

export default Cow;