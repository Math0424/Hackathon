import {Component} from 'react';
import { Router, Switch, Route } from "react-router-dom";
import Login from './views/login';

export default class Routes extends Component {
    render() {
        return (
            <Router history={history}>
                <Switch>
                    <Route path="/" exact component={Login} />
                    <Route path="/Cow" component={Cow} />
                </Switch>
            </Router>
        )
    }
}