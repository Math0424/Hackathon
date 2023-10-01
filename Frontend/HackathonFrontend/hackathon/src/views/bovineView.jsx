import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import Cookies from 'js-cookie';

const useQuery = () => new URLSearchParams(useLocation().search);

const getCowById = async (id) => {
  const response = await fetch(`http://173.215.25.174:5000/bovine/get?id=${id}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      "Authorization": "Bearer " + Cookies.get("userId"),
    },
  });
  return response.json();
};

const BovineView = () => {
  const [data, setData] = useState(null);
  const query = useQuery();
  const id = query.get('id');

  useEffect(() => {
    getCowById(id)
      .then((json) => setData(json))
      .catch((error) => console.error("Fetch error: ", error));
  }, [id]);

  return (
    <div>
      {data && (
        <>
          <div>ID: {data.id}</div>
          <div>Owner ID: {data.ownerId}</div>
          <div>Name: {data.name}</div>
          <div>Male: {data.male ? 'Yes' : 'No'}</div>
          <div>Death: {data.death}</div>
          <div>Cull: {data.cull}</div>
          <div>Culled: {data.culled ? 'Yes' : 'No'}</div>
          <div>Castrated: {data.castrated ? 'Yes' : 'No'}</div>
        </>
      )}
    </div>
  );
};

export default BovineView;
