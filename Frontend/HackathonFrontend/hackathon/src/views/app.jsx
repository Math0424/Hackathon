import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import Cow from './Cow';
import Login from './login';
import CowView from './cowview';
import BovineView from './bovineView';

const App = () => {
    return (
        <div>
            <Router>
                <Routes>
                    <Route path="/" element={<Login />} />
                    <Route path="/Cow" element={<Cow />} />
                    <Route path="/CowView" element={<CowView />} />
                    <Route path="/BovineView" element={<BovineView />} />
                </Routes>
            </Router>
        </div>
    );
};

export default App;
