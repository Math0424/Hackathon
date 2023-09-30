import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import Login from './views/login';
import Cow from './views/Cow';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <React.StrictMode>
        <Login />
        <Routes />
    </React.StrictMode>
);
