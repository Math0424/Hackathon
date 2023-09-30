import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import Login from './views/login';
import Cow from './views/Cow';
import {Router} from 'react-router-dom';
import App from './views/app';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <React.StrictMode>

        <App/>
    </React.StrictMode>
);
