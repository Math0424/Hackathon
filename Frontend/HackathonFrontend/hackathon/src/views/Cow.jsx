import { useState } from "react";
import Cookies from 'js-cookie';
import { useNavigate } from 'react-router-dom';

const Cow = () => {
const [name, setname] = useState("");
const [gender, setgender] = useState("");
const [died, setdied] = useState("");
const [cull, setcull] = useState("");
const [culled, setculled] = useState("");
const [casterated, setcastereated] = useState("");
const [error, setError] = useState(null);

const navigate = useNavigate();
const handleSubmit = (e) => {
    e.preventDefault();
      // Create the new cow object
      const newCow = {
        name,
        male: gender === "true",
        died,
        cull: cull === "true",
        culled: culled === "true",
        casterated: casterated === "true",
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
            <form onSubmit={handleSubmit}>
            <p> Name: (name)
                <input
                     type="text"
                     name="name"
                     value={name}
                     onChange={(e) => setname(e.target.value)}
                />
            </p>
            <p> Gender: (Male)
                <input
                    type="checkbox"
                    name="gender"
                    checked={gender === "male"}
                    onChange={() => setgender(gender === "male" ? "female" : "male")}
                />
            </p>
            <p> Died: (d/t)
                    <input
                       type="text"
                       name="died"
                       value={died}
                        onChange={(e) => setdied(e.target.value)}
                       />
            </p>
            <p> ToCull:
            <input
                type="checkbox"
                name="cull"
                checked={cull === "true"}
                onChange={() => setcull(cull === "true" ? "false" : "true")}
            />
            </p>
            <p> Culled:
            <input
                type="checkbox"
                name="culled"
                checked={culled === "true"}
                onChange={() => setculled(culled === "true" ? "false" : "true")}
            />
            </p>
            <p> Castrated:
            <input
                type="checkbox"
                name="casterated"
                checked={casterated === "true"}
                onChange={() => setcastereated(casterated === "true" ? "false" : "true")}
            />
            </p>
            <button type="submit">Create</button>
            {error && <div className="error">{error}</div>}
            </form>
            
        </div>
    )
}

export default Cow;