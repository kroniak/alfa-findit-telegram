import React, {Component} from 'react';
import Heading from 'arui-feather/heading';
import {connect} from "react-redux";
import {fetchUsers} from "../actions/users";
import Table from "./table";
import Button from "arui-feather/button";
import Spin from "arui-feather/spin";

class Users extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLoading: false
        }
    }

    componentWillReceiveProps(nextProps, nextContext) {
        if (nextProps.isLoading) {
            this.setState({
                isLoading: true
            });
            setTimeout(() => {
                this.setState({
                    isLoading: false
                });
            }, 500);
        }
    }

    componentDidMount() {
        this.props.fetchUsers();
    }

    renderData = users => {
        if (users) return <Table users={users}/>;
        else return null;
    };

    render() {
        const {users} = this.props;
        return (
            <div>
                <Heading size='m'>
                    Список пользователей
                </Heading>
                <Button view='extra'
                        size='m'
                        onClick={this.props.fetchUsers}>
                    Обновить
                </Button>
                <Spin
                    className="button-spin"
                    size='m'
                    visible={this.state.isLoading}
                />
                {this.renderData(users)}
            </div>
        );
    }
}

const mapStateToProps = state => ({
    users: state.users.data,
    error: state.users.error,
    isLoading: state.users.isLoading
});

const mapDispatchToProps = dispatch => ({
    fetchUsers: () => dispatch(fetchUsers()),
});

export default connect(mapStateToProps, mapDispatchToProps)(Users);