import React, {Component} from 'react';
import Tabs from 'arui-feather/tabs';
import TabItem from 'arui-feather/tab-item';
import Results from './results';
import Users from "./users";
import {connect} from "react-redux";
import {logUser, verifyToken} from "../actions/auth";
import Authenticated from "./authenticated";
import LoginForm from "./login_form";
import {changeTab, loadTab} from "../actions/view";

class App extends Component {
    componentDidMount() {
        this.props.verifyAuth();
        this.props.loadTab();
    };

    handleClick = event => {
        event.preventDefault();
        this.props.onTabChange(event.target.getAttribute('href'));
    };

    renderContent = () => {
        const {page, isAuth, onLogUserClick, authError} = this.props;

        if (page === '/results') {
            return <Results/>
        }
        if (page === '/users') {
            return (
                <div>
                    <Authenticated isAuth={isAuth}>
                        <Users/>
                    </Authenticated>
                    {!isAuth &&
                    <LoginForm logUser={onLogUserClick} authError={authError}/>}
                </div>
            )
        }
    };

    render() {
        const {page} = this.props;
        return (
            <div className="app">
                <header className="header">
                    <img src={"img/icon_bank.svg"} alt="logo"/>
                    <h2 className="header-name"> Альфа-Банк Викторина</h2>
                </header>
                <main>
                    <Tabs>
                        <TabItem url='/results'
                                 onClick={this.handleClick}
                                 checked={page === '/results'}>
                            Результаты
                        </TabItem>
                        <TabItem url='/users' onClick={this.handleClick} checked={page === '/users'}>
                            Пользователи
                        </TabItem>
                    </Tabs>
                    {this.renderContent()}
                </main>
            </div>
        );
    }
}

const mapsStateToProps = state => ({
    isAuth: state.auth.isAuth,
    authError: state.auth.error,
    page: state.view.activeTab
});

const mapDispatchToProps = dispatch => ({
    verifyAuth: () => dispatch(verifyToken()),
    onLogUserClick: (user, pass) => dispatch(logUser(user, pass)),
    onTabChange: page => dispatch(changeTab(page)),
    loadTab: () => dispatch(loadTab())
});

export default connect(mapsStateToProps, mapDispatchToProps)(App);
