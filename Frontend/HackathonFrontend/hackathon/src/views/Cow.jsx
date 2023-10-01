import { useState } from "react";
import Cookies from 'js-cookie';
import { useNavigate } from 'react-router-dom';
import styles from './Cow.module.scss';

const Cow = () => {
const [name, setname] = useState("");
const [gender, setGender] = useState(false);
const [cull, setCull] = useState(false);
const [culled, setCulled] = useState(false);
const [castrated, setCastrated] = useState(false);
const [error, setError] = useState(null);

    const navigate = useNavigate();
    const handleSubmit = (e) => {
        e.preventDefault();
        // Create the new cow object
        const newCow = {
            name,
            male: gender,
            cull,
            culled,
            castrated,
        };

        // Send POST request
        fetch('http://173.215.25.174:5000/bovine/create', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                "Authorization": "Bearer " + Cookies.get("userId"),
            },
            body: JSON.stringify(newCow),
        })
            .then(response => {
                console.log('Raw Response:', response);
                return response.json();
            })
            .then(data => {
                console.log('Success:', data);
                navigate("/cowview")
            })
            .catch((error) => {
                console.error('Error:', error);
                setError("An error occurred while creating the cow. \n" + error);
            });
    };

    return (
        <div>
            <h1>Cow: (id)</h1>
            <form className={styles.list} onSubmit={handleSubmit}>
                <p>
                    <span>Name: (name)</span>
                    <input
                        type="text"
                        name="name"
                        value={name}
                        onChange={(e) => setname(e.target.value)}
                    />
                </p>
                <p>
                    <span>Gender: (Male)</span>
                    <input
                        type="checkbox"
                        name="gender"
                        checked={gender}
                        onChange={() => setGender(!gender)}
                    />
                </p>
                <p>
                    <span>ToCull:</span>
                    <input
                        type="checkbox"
                        name="cull"
                        checked={cull}
                        onChange={() => setCull(!cull)}
                    />
                </p>
                <p>
                    <span>Culled:</span>
                    <input
                        type="checkbox"
                        name="culled"
                        checked={culled}
                        onChange={() => setCulled(!culled)}
                    />
                </p>
                <p>
                    <span>Castrated:</span>
                    <input
                        type="checkbox"
                        name="castrated"
                        checked={castrated}
                        onChange={() => setCastrated(!castrated)}
                    />
                </p>
            </form>
            <button type="submit">Create</button>
            {error && <div className="error">{error}</div>}

        </div>
    )
}

export default Cow;