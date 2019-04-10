import React, {Component} from 'react';
import Tabs from 'arui-feather/tabs';
import TabItem from 'arui-feather/tab-item';
import Results from './results';
import Users from "./users";

class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            page: "/results"
        };

        this.handleClick = this.handleClick.bind(this);
        this.renderContent = this.renderContent.bind(this);
    }

    handleClick(event) {
        event.preventDefault();
        this.setState({
            page:
                event.target.getAttribute('href')
        });
    }

    renderContent() {
        if (this.state.page === '/results') {
            return (
                <Results/>
            )
        }
        if (this.state.page === '/users') {
            return (
                <Users/>
            )
        }
    }

    render() {
        return (
            <div className="app">
                <header className="header">
                    <h2 className="header-name"> Альфа-Банк Викторина</h2>
                </header>
                <main>
                    <Tabs>
                        <TabItem url='/results'
                                 onClick={this.handleClick}
                                 checked={this.state.page === '/results'}>
                            Результаты
                        </TabItem>
                        <TabItem url='/users' onClick={this.handleClick} checked={this.state.page === '/users'}>
                            Пользователи
                        </TabItem>
                    </Tabs>
                    {this.renderContent()}
                </main>
            </div>
        );
    }
}

export default App;
