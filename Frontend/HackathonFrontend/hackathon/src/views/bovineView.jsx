import React, { useState, useEffect } from 'react';
import { useLocation } from 'react-router-dom';
import Cookies from 'js-cookie';
import styles from './bovineView.module.scss'
import ReactDOM from "react-dom";
import QRCode from "react-qr-code";
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
    <div className={styles.list}>
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
          <div><QRCode value={`http://173.215.25.174:5000/bowvineView/id=${id}`}   size={150}
                                                                                      style={{ height: "auto", maxWidth: "100%", width: "100%" }}

                                                                                      viewBox={`0 0 256 256`}/></div>
        </>
      )}

    </div>
  );
};

export default BovineView;
