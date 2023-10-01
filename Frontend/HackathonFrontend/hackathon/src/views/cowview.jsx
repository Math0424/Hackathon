import React, { useState, useEffect } from 'react';
import Cookies from 'js-cookie';
import { Link } from 'react-router-dom';

const getAllCows = async () => {
    const response = await fetch('http://localhost:5000/bovine/getall', {
        method: "GET",
        mode: "cors",
        cache: "no-cache",
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer " + Cookies.get("userId"),
        },
        redirect: "follow",
        referrerPolicy: "no-referrer",
    });
    return response.json();
}

const CowView = () => {
  const [data, setData] = useState([]);

  useEffect(() => {
    getAllCows()
    .then((json) => setData(json))
    .catch((error) => console.error("Fetch error: ", error));
  }, []);

  return (
    <div>
      <Link to="/Cow">
        <button>+</button>
      </Link>
      <div style={{ overflowY: 'auto', height: '400px' }}>
        <ul>
          {
            data.map((item) => (
              <li key={item.id}>
                {`ID: ${item.id}, Name: ${item.name}, Gender: ${item.male ? "male" : "female"}`}
                <br />
                <Link to={`/bovineView?id=${item.id}`}>
                    <button>View Details</button>
                </Link>

              </li>
            ))
          }
        </ul>
      </div>
    </div>
  );
};

export default CowView;
