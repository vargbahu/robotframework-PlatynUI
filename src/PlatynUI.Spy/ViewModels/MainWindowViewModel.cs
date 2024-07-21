// SPDX-FileCopyrightText: 2024 Daniel Biehl <daniel.biehl@imbus.de>
//
// SPDX-License-Identifier: Apache-2.0

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Metadata;
using Avalonia.Threading;

using PlatynUI.Runtime;
using PlatynUI.Runtime.Core;

using ReactiveUI;

namespace PlatynUI.Spy.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public void Refresh()
    {
        SelectedNode?.Refresh();
    }

    [DependsOn(nameof(SelectedNode))]
    public bool CanRefresh()
    {
        return SelectedNode != null;
    }

    public TreeNode[] Root { get; } = [new TreeNode(null, Desktop.Instance) { IsExpanded = true, IsSelected = true }];

    private TreeNode? _selectedNode;
    public TreeNode? SelectedNode
    {
        get => _selectedNode;
        set => this.RaiseAndSetIfChanged(ref _selectedNode, value);
    }

    private TreeNode? _resultsSelectedNode;
    public TreeNode? ResultsSelectedNode
    {
        get => _resultsSelectedNode;
        set
        {
            if (value != null)
            {
                var treeNode = FindNodeInTree(value.Node);
                if (treeNode != null)
                {
                    SelectedNode = null;

                    foreach (var ancestor in treeNode.GetAncestors())
                    {
                        ancestor.IsExpanded = true;
                    }

                    treeNode.IsSelected = true;
                }
            }
            this.RaiseAndSetIfChanged(ref _resultsSelectedNode, value);
        }
    }

    private string? _searchText;
    public string? SearchText
    {
        get => _searchText;
        set => this.RaiseAndSetIfChanged(ref _searchText, value);
    }

    public void CancelSearch()
    {
        if (_searchCancellationTokenSource != null)
        {
            _searchCancellationTokenSource.Cancel();
            _searchCancellationTokenSource = null;
            InSearch = false;
        }
        else
        {
            Debug.WriteLine("No search in progress");
        }
    }

    private bool _inSearch;
    public bool InSearch
    {
        get => _inSearch;
        set => this.RaiseAndSetIfChanged(ref _inSearch, value);
    }

    private TreeNode? FindNodeInTree(INode node)
    {
        var parents = node.GetAncestors();

        var currentTreeNode = Root[0];

        if (parents.Count > 0 && parents[0].IsSamePosition(Root[0].Node))
        {
            parents.RemoveAt(0);
        }
        foreach (var parentNode in parents)
        {
            var foundTreeNode = currentTreeNode.FindNode(parentNode);

            if (foundTreeNode == null)
            {
                break;
            }

            currentTreeNode = foundTreeNode;
        }

        if (currentTreeNode != null)
        {
            return currentTreeNode!.FindNode(node);
        }

        return currentTreeNode;
    }

    CancellationTokenSource? _searchCancellationTokenSource;

    public async void Search()
    {
        CancelSearch();

        SearchResults.Clear();
        if (string.IsNullOrEmpty(SearchText))
            return;

        _searchCancellationTokenSource = new();
        var token = _searchCancellationTokenSource.Token;
        InSearch = true;
        LastError = "";
        try
        {
            await Task.Run(
                async () =>
                {
                    var first = true;

                    foreach (var node in Finder.EnumAllNodes(Desktop.Instance, SearchText))
                    {
                        token.ThrowIfCancellationRequested();

                        var treeNode = Root.FirstOrDefault(n => n.Node.IsSamePosition(node)) ?? FindNodeInTree(node);
                        //var treeNode = new TreeNode(null, node);

                        if (treeNode != null)
                        {
                            await Dispatcher.UIThread.InvokeAsync(
                                () =>
                                {
                                    token.ThrowIfCancellationRequested();
                                    SearchResults.Add(treeNode);
                                },
                                DispatcherPriority.Normal,
                                token
                            );

                            if (first)
                            {
                                first = false;
                                await Dispatcher.UIThread.InvokeAsync(
                                    () =>
                                    {
                                        token.ThrowIfCancellationRequested();
                                        ResultsSelectedNode = treeNode;
                                    },
                                    DispatcherPriority.Normal,
                                    token
                                );
                            }
                        }
                    }
                },
                token
            );
        }
        catch (Exception ex)
        {
            LastError = ex.Message;
            Debug.WriteLine(ex);
        }
        finally
        {
            InSearch = false;
        }
    }

    public ObservableCollection<TreeNode> SearchResults { get; } = [];

    private string _lastError = "";
    public string LastError
    {
        get => _lastError;
        set => this.RaiseAndSetIfChanged(ref _lastError, value);
    }
}
