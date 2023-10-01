import { useState } from "react";
import Cookies from 'js-cookie';
import { useNavigate } from 'react-router-dom';

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
              checked={gender}
              onChange={() => setGender(!gender)}
            />
            </p>
            <p> ToCull:
            <input
              type="checkbox"
              name="cull"
              checked={cull}
              onChange={() => setCull(!cull)}
            />
            </p>
            <p> Culled:
            <input
              type="checkbox"
              name="culled"
              checked={culled}
              onChange={() => setCulled(!culled)}
            />
            </p>
            <p> Castrated:
            <input
              type="checkbox"
              name="castrated"
              checked={castrated}
              onChange={() => setCastrated(!castrated)}
            />
            </p>
            <button type="submit">Create</button>
            {error && <div className="error">{error}</div>}
            </form>
            
        </div>
    )
}

export default Cow;